using System.Runtime.CompilerServices;
using td.utils;
using UnityEngine;

namespace td.features._common
{
    public static class CommonUtils
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string IntegerFormat(uint number) => $"{number:N0}"
            .Replace(',', '\'').Replace(' ', '\'');

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string IntegerFormat(int number) => $"{number:N0}"
            .Replace(',', '\'').Replace(' ', '\'');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string IntegerFormat(uint number, bool _short)
        {
            if (!_short) return IntegerFormat(number);

            var s = "";
            var f = 0f;
            if (number >= 1000000000) {
                s = "B";
                f = (number / 1_000_000) / 100f;
            } else if (number >= 1_000_000) {
                s = "M";
                f = (number / 10_000) / 100f;
            }/* else if (number >= 1000) {
                s = "K";
                f = number / 1_000f;
            }*/

            if (string.IsNullOrEmpty(s)) {
                return IntegerFormat(number);
            }

            var f100 = (int)(f * 100);
            var p1 = f100 / 100;
            var p2 = f100 - (p1 * 100);

            return IntegerFormat(p1) + $".{p2:D2}{s}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TimeFormat(uint number)
        {
            var s = number % 60;
            var m = number / 60;
            var h = number / 3600;

            if (h == 0 && m == 0) return s.ToString();
            if (h == 0 && m > 0) return $"{m}:{s:D2}";
            if (h > 0 && m > 0) return $"{h}:{m:D2}:{s:D2}";
            return $"{number}s";
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string PriceFormat(uint cost) =>
            $"<size=80%>{Constants.UI.CurrencySign}</size>{IntegerFormat(cost)}";

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static string TMPTimeFormat(uint time) => $"<size=80%><sprite=0 tint></size>{TimeFormat(time)}";

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