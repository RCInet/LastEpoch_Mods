using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IIOProxy : IService
    {
        // Directory
        bool DirectoryExists(string directoryPath);
        void DirectoryDelete(string path, bool recursive);
        void CreateDirectory(string directoryPath);
        IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
        bool DeleteAllFilesAndFoldersFromDirectory(string path);

        // Directory Info
        string GetDirectoryInfoFullName(string path);
        double GetDirectorySizeBytes(string folderPath);
        string GetUniqueTempPathInProject();

        // File
        bool FileExists(string filePath);
        void DeleteFile(FileInfo file);
        void DeleteFile(string filePath, bool recursivelyRemoveEmptyParentFolders = false);
        FileStream Create(string path, int bufferSize, FileOptions options);
        void FileMove(string sourceFilePath, string destinationFilePath);
        string FileReadAllText(string filePath);
        void FileWriteAllText(string filePath, string text);
        byte[] FileReadAllBytes(string filePath);
        void FileWriteAllBytes(string filePath, byte[] bytes);
        Stream FileOpenRead(string path);

        // File Info
        DateTime GetFileLastWriteTimeUtc(string path);
        long GetFileLength(string path);
        double GetFileLengthMb(string filePath);
        double GetFileLengthMb(FileInfo file);
        double GetFilesSizeMb(IEnumerable<FileInfo> files);
        IEnumerable<FileInfo> GetOldestFilesFromDirectory(string directoryPath);
    }

    [Serializable]
    class IOProxy : BaseService<IIOProxy>, IIOProxy
    {
        // Directory
        public bool DirectoryExists(string directoryPath) => Directory.Exists(directoryPath);

        public void DirectoryDelete(string path, bool recursive)
        {
            if (DirectoryExists(path))
            {
                Directory.Delete(path, recursive);
            }
        }

        public void CreateDirectory(string directoryPath) => Directory.CreateDirectory(directoryPath);

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFiles(path, searchPattern, searchOption);

        public bool DeleteAllFilesAndFoldersFromDirectory(string path)
        {
            if (!DirectoryExists(path))
            {
                return false;
            }
            
            var directory = new DirectoryInfo(path);
            var success = true;

            foreach (var file in directory.EnumerateFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (IOException)
                {
                    success = false;
                }
            }

            foreach (var directoryInfo in directory.EnumerateDirectories())
            {
                try
                {
                    directoryInfo.Delete(true);
                }
                catch (IOException)
                {
                    success = false;
                }
            }

            return success;
        }

        // Directory Info
        public string GetDirectoryInfoFullName(string path)
        {
            if (!Directory.Exists(path))
            {
                return string.Empty;
            }

            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.FullName;
        }

        public double GetDirectorySizeBytes(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return 0;
            }

            var directoryInfo = new DirectoryInfo(folderPath);
            return directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        static bool DirectoryEmpty(DirectoryInfo directoryInfo) => !directoryInfo.EnumerateFiles().Any() && !directoryInfo.EnumerateDirectories().Any();

        public string GetUniqueTempPathInProject() => FileUtil.GetUniqueTempPathInProject();

        // File
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public void DeleteFile(FileInfo file)
        {
            if (!file.Exists || IsFileLocked(file))
            {
                return;
            }

            try
            {
                file.Delete();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public void DeleteFile(string filePath, bool recursivelyRemoveEmptyParentFolders = false)
        {
            if (!File.Exists(filePath))
                return;

            DeleteFile(new FileInfo(filePath));

            if (!recursivelyRemoveEmptyParentFolders)
                return;

            var parentFolder = Directory.GetParent(filePath);
            while (parentFolder?.Exists == true && DirectoryEmpty(parentFolder))
            {
                DirectoryDelete(parentFolder.FullName, false);
                parentFolder = parentFolder.Parent;
            }
        }

        public FileStream Create(string path, int bufferSize, FileOptions options) => File.Create(path, bufferSize, options);

        public void FileMove(string sourceFilePath, string destinationFilePath)
        {
            if (!FileExists(sourceFilePath))
                return;

            new FileInfo(destinationFilePath).Directory?.Create();
            File.Move(sourceFilePath, destinationFilePath);
        }

        public string FileReadAllText(string filePath) => File.ReadAllText(filePath);

        public void FileWriteAllText(string filePath, string text) => File.WriteAllText(filePath, text);
        
        public byte[] FileReadAllBytes(string filePath) => File.ReadAllBytes(filePath);

        public void FileWriteAllBytes(string filePath, byte[] bytes) => File.WriteAllBytes(filePath, bytes);
        
        public Stream FileOpenRead(string path) => File.OpenRead(path);

        // File Info
        public DateTime GetFileLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);
        
        public long GetFileLength(string path)
        {
            if (!File.Exists(path))
            {
                return 0;
            }

            var fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        public double GetFileLengthMb(string filePath)
        {
            return ByteSizeConverter.ConvertBytesToMb(GetFileLength(filePath));
        }

        public double GetFileLengthMb(FileInfo file)
        {
            return ByteSizeConverter.ConvertBytesToMb(file.Length);
        }

        public double GetFilesSizeMb(IEnumerable<FileInfo> files)
        {
            return ByteSizeConverter.ConvertBytesToMb(files.Sum(x => x.Length));
        }

        public IEnumerable<FileInfo> GetOldestFilesFromDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                return new List<FileInfo>();
            }

            return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(x => x.LastAccessTimeUtc <= DateTime.UtcNow.AddMinutes(1))
                .OrderByDescending(x => x.LastAccessTimeUtc);
        }

    }
}
