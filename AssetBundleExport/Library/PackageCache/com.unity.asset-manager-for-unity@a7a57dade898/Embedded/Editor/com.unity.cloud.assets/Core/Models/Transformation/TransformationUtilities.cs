namespace Unity.Cloud.AssetsEmbedded
{
    static class TransformationUtilities
    {
        internal static string GetValue(bool? value)
        {
            return value.HasValue ? GetValue(value.Value) : null;
        }

        static string GetValue(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
