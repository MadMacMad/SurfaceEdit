using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public static class MiscUtility
    {
        private static Dictionary<Type, int> enumCounts = new Dictionary<Type, int> ();

        public static int EnumCount<T> () where T : struct, IConvertible
        {
            var type = typeof (T);

            Assert.ArgumentTrue (type.IsEnum, type.Name + " is not enum");

            if ( !enumCounts.ContainsKey (type) )
            {
                var count = Enum.GetValues (type).Length;
                enumCounts.Add (type, count);
                return count;
            }

            return enumCounts[type];
        }
    }
}
