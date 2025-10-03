using System.Runtime.Serialization;

namespace Unity.Cloud.CommonEmbedded
{
    [DataContract]
    struct FileDescriptorDto
    {
        [DataMember(Name = "datasetDescriptor")]
        public DatasetDescriptorDto DatasetDescriptor { get; set; }

        [DataMember(Name = "filePath")]
        public string FilePath { get; set; }

        public FileDescriptorDto(FileDescriptor fileDescriptor)
        {
            DatasetDescriptor = new DatasetDescriptorDto(fileDescriptor.DatasetDescriptor);
            FilePath = fileDescriptor.Path;
        }
    }
}
