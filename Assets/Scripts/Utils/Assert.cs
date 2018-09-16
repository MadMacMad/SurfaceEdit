using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilify
{
    public static class Assert
    {
        public static void ArgumentTrue(bool value, string message)
        {
            if ( !value )
                throw new ArgumentException (message);
        }
        public static void ArgumentNotNull<T>(T obj, string name) where T : class
        {
            if ( obj is null || obj == null )
                throw new ArgumentNullException (name, name + " is null");
        }
    }
}
