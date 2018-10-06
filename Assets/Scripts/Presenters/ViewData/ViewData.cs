using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SurfaceEdit.Presenters
{
    public abstract class ViewData : MonoBehaviour
    {
        private void Start ()
        {
            var type = GetType ();

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if ( field.FieldType.IsValueType )
                    continue;

                var value = field.GetValue (this);

                if ( value == null )
                    throw new Exception ($"ViewData of type {type.Name} contains field of type {field.FieldType.Name} named {field.Name} with null value! " +
                                         $"ViewData should not contains fields with null value!");

                if (value is UnityEngine.Object unityObj)
                    if ( unityObj == null)
                        throw new Exception ($"ViewData of type {type.Name} contains field of type {field.FieldType.Name} named {field.Name} with null value! " +
                                             $"ViewData should not contains fields with null value!");

                if ( value is IEnumerable<object> enumerable)
                {
                    if ( enumerable.Count () == 0 )
                        throw new Exception ($"ViewData of type {type.Name} contains field of type {field.FieldType.Name} " +
                                             $"named {field.Name} that is IEnumerable<> and it is emptry! " +
                                             $"ViewData should not contains empty IEnumerable<> fields!");

                    foreach ( var obj in enumerable )
                        if ( obj == null )
                            throw new Exception ($"ViewData of type {type.Name} contains field of type {field.FieldType.Name} " +
                                             $"named {field.Name} that is IEnumerable<> and it contains null object(s)! " +
                                             $"ViewData should not contains IEnumerable<> fields with nulls in it!");
                }
            }
        }
    }
}