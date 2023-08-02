using UnityEngine;

namespace td.features._common
{
    public static class CommonUtils
    {
        public static string CostFormat(uint cost) => $"<size=80%>{Constants.UI.CurrencySign}</size>{cost:N0}"
            .Replace(',', '\'').Replace(' ', '\'');

        private static uint lastId;

        public static uint ID(string type = null)
        {
            lastId++;
            // Debug.Log("> ID: " + lastId + " - " + type);
            return lastId;
        }

        public static bool IdsIsEquals(uint id1, uint id2) => id1 > 0 && id1 == id2;
    }
}