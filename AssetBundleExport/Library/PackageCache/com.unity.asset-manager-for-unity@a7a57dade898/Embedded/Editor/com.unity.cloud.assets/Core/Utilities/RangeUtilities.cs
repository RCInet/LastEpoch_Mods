using System;

namespace Unity.Cloud.AssetsEmbedded
{
    static class RangeUtilities
    {
        internal static (int, int) GetValidatedOffsetAndLength(this Range range, int collectionLength)
        {
            int start;
            int length;

            try
            {
                (start, length) = range.GetOffsetAndLength(collectionLength);
            }
            catch (ArgumentOutOfRangeException)
            {
                start = range.Start.IsFromEnd ? Math.Max(0, collectionLength - range.Start.Value) : Math.Min(collectionLength, range.Start.Value);
                var end = range.End.IsFromEnd ? Math.Max(0, collectionLength - range.End.Value) : Math.Min(collectionLength, range.End.Value);
                length = Math.Clamp(end - start, 0, collectionLength);
            }

            return (start, length);
        }
    }
}
