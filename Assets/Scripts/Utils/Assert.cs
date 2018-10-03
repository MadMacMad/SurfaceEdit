﻿using System;
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
        public static void ArgumentNotNull<T>(T obj, string name) where T : class
        {
            if ( obj is null || obj == null )
                throw new ArgumentNullException (name, name + " is null");
        }
    }
}
