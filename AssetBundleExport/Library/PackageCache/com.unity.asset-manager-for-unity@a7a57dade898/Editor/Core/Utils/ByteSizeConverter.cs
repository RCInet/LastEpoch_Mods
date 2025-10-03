using System;

namespace Unity.AssetManager.Core.Editor
{
    static class ByteSizeConverter
    {
        const double m_BytesToMb = 1_048_576; // 1024 * 1024;
        const double m_BytesToGb = 1_073_741_824; // 1024 * 1024 * 1024;

        /// <summary>
        /// Converts bytes to Mbs
        /// </summary>
        /// <param name="bytes">the bytes to be converted</param>
        /// <returns>the converted megabytes</returns>
        public static double ConvertBytesToMb(double bytes)
        {
            return bytes <= 0 ? 0 : bytes / m_BytesToMb;
        }

        /// <summary>
        /// Converts bytes to Gbs
        /// </summary>
        /// <param name="bytes">the bytes to be converted</param>
        /// <returns>the converted Gb</returns>
        public static double ConvertBytesToGb(double bytes)
        {
            return bytes <= 0 ? 0 : bytes / m_BytesToGb;
        }
    }
}
