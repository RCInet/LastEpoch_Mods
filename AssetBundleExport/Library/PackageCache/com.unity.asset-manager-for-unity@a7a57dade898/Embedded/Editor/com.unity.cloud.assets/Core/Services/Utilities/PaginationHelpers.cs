using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    static class PaginationHelpers
    {
        internal delegate Task<int> GetTotalCount(CancellationToken cancellationToken);

        internal static async Task<(int Offset, int Length)> GetOffsetAndLengthAsync(this Range range, GetTotalCount getTotalCount, CancellationToken cancellationToken)
        {
            int offset;
            int length;

            if (range.Start.IsFromEnd || (range.End.IsFromEnd && range.End.Value != 0))
            {
                var count = await getTotalCount(cancellationToken);

                offset = CheckIndex(range.Start, count);
                var endIndex = CheckIndex(range.End, count);

                length = Math.Max(0, endIndex - offset);
            }
            else
            {
                offset = range.Start.Value;

                if (range.End.Equals(Index.End))
                {
                    length = int.MaxValue - range.Start.Value;
                }
                else
                {
                    length = Math.Max(0, range.End.Value - range.Start.Value);
                }
            }

            return (offset, length);
        }

        static int CheckIndex(Index index, int totalLength)
        {
            return index.IsFromEnd
                ? Math.Max(0, totalLength - index.Value)
                : index.Value;
        }
    }
}
