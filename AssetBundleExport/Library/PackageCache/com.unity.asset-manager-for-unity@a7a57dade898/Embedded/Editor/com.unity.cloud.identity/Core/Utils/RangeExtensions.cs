using System;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// Helper methods for <see cref="System.Range"/>.
    /// </summary>
    internal static class RangeExtensions
    {
        public static Range NormalizeRange(this Range range, int max)
        {
            if (range.Equals(Range.All))
            {
                return new Range(0, max);
            }
            var startIndex = range.Start.IsFromEnd ? Math.Max(0, max - range.Start.Value) : Math.Min(max, range.Start.Value);
            var endIndex = range.End.IsFromEnd ? Math.Max(0, max - range.End.Value) : Math.Min(max, range.End.Value);
            return new Range(startIndex, endIndex);
        }
    }
}
