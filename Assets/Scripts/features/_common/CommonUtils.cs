using System.Runtime.CompilerServices;

namespace td.features._common
{
    public static class CommonUtils
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string IntegerFormat(uint number) => $"{number:N0}"
            .Replace(',', '\'').Replace(' ', '\'');

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string PriceFormat(uint cost) =>
            $"<size=80%>{Constants.UI.CurrencySign}</size>{IntegerFormat(cost)}";

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string TMPTimeFormat(uint time) => $"<size=80%><sprite=0 tint></size>{IntegerFormat(time)}";

        private static uint lastId;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static uint ID(string type = null)
        {
            lastId++;
            // Debug.Log("> ID: " + lastId + " - " + type);
            return lastId;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IdsIsEquals(uint id1, uint id2) => id1 > 0 && id1 == id2;
    }
}