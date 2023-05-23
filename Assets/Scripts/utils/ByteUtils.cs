namespace td.utils
{
    public static class ByteUtils
    {
        public static byte Min(params byte[] values)
        {
            var length = values.Length;
            if (length == 0) return 0;
            var num = values[0];
            for (var index = 1; index < length; ++index)
            {
                if (values[index] < num)
                {
                    num = values[index];
                }
            }
            return num;
        }
        
        public static byte Max(params byte[] values)
        {
            var length = values.Length;
            if (length == 0) return 0;
            var num = values[0];
            for (var index = 1; index < length; ++index)
            {
                if (values[index] > num)
                {
                    num = values[index];
                }
            }
            return num;
        }
    }
}