using System;
using UnityEngine;

namespace SurfaceEdit
{
    public static class Assert
    {
        public static void ArgumentTrue(bool value, string message)
        {
            if ( !value )
                throw new ArgumentException (message);
        }
        public static void True(bool value, string message)
        {
            if ( !value )
                throw new Exception (message);
        }
        public static void SoftTrue(bool value, string message)
        {
            if ( !value )
                Debug.LogError (message);
        }
        public static void SoftNotNull<T> (T obj, string name) where T : class
        {
            if ( obj is null || obj == null )
                Debug.LogError (name + " is null");
        }
        public static void NotNull<T> (T obj, string name) where T : class
        {
            if ( obj is null || obj == null )
                throw new Exception (name + " is null");
        }

        public static void ArgumentNotNullOrEmptry (string text, string name)
        {
            if ( string.IsNullOrEmpty (text) )
                throw new ArgumentException (name + " is null or emptry", name);
        }

        public static void ArgumentNotNull<T>(T obj, string name) where T : class
        {
            if ( obj is null || obj == null )
                throw new ArgumentNullException (name, name + " is null");
        }
    }
}
