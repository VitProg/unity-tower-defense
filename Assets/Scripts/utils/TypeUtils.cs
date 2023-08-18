using System;
using System.Runtime.CompilerServices;

namespace td.utils
{
    public static class TypeUtils
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool HasInterface(Type type, Type interfaceType)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (i == interfaceType) return true;
                break;
            }

            return false;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool HasInterface(Type type, string interfaceName) => type.GetInterface(interfaceName) != null;
    }
}