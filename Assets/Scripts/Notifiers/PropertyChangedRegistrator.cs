﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SurfaceEdit.Commands;

namespace SurfaceEdit
{
    public abstract class PropertyChangedRegistrator : ObjectChangedNotifier
    {
        public readonly UndoRedoManager undoRedoManager;

        public PropertyChangedRegistrator(UndoRedoManager undoRedoManager)
        {
            Assert.ArgumentNotNull (undoRedoManager, nameof (undoRedoManager));
            this.undoRedoManager = undoRedoManager;
        }

        /// <summary>
        /// Modifies and validates the new value with validator.
        /// If the validation is not successful, nothing happens.
        /// Otherwise, if the new value is not equal to the current value:
        /// 1. It creates a new undo/redo SetPropertyCommand and registers it in undoRedoManager.
        /// 2. Each time the property is changed by the SetPropertyCommand, NotifyPropertyChanged will be executed.
        /// 3. If notifyObjectChanged is set to true, each time the property is changed by the SetPropertyCommand, NotifyObjectChanged will be executed.
        /// </summary>
        protected void SetPropertyUndoRedoValidate<T> (Action<T> setter, Func<T> getter, T newValue,
                                       bool notifyObjectChanged = false,
                                       Func<T, Tuple<bool, T>> validator = null,
                                       [CallerFilePath] string pathName = "",
                                       [CallerMemberName] string propertyName = "")
        {
            Assert.ArgumentNotNull (setter, nameof (setter));
            Assert.ArgumentNotNull (getter, nameof (getter));

            if ( validator != null )
            {
                var result = validator (newValue);
                if ( !result.Item1 )
                    return;
                newValue = result.Item2;
            }

            if ( !EqualityComparer<T>.Default.Equals (getter(), newValue) )
            {
                Action callback = () => NotifyPropertyChanged(propertyName);

                if (notifyObjectChanged)
                    callback += () => NotifyChanged ();

                var command = new SetPropertyCommand<T> (new Ref<T>(setter, getter), newValue, callback, pathName, propertyName);
                undoRedoManager.Do (command as ICommand);
            }
        }

        /// <summary>
        /// Modifies the new value with modifier.
        /// If the new value is not equal to the current value:
        /// 1. It creates a new undo/redo SetPropertyCommand and registers it in undoRedoManager.
        /// 2. Each time the property is changed by the SetPropertyCommand, NotifyPropertyChanged will be executed.
        /// 3. If notifyObjectChanged is set to true, each time the property is changed by the SetPropertyCommand, NotifyObjectChanged will be executed.
        /// </summary>
        protected void SetPropertyUndoRedo<T> (Action<T> setter, Func<T> getter, T newValue,
                                       bool notifyObjectChanged = false,
                                       Func<T, T> modifier = null,
                                       [CallerFilePath] string pathName = "",
                                       [CallerMemberName] string propertyName = "")
        {
            Assert.ArgumentNotNull (setter, nameof (setter));
            Assert.ArgumentNotNull (getter, nameof (getter));

            if ( modifier != null )
                newValue = modifier (newValue);

            if ( !EqualityComparer<T>.Default.Equals (getter (), newValue) )
            {
                Action callback = () => NotifyPropertyChanged (propertyName);

                if ( notifyObjectChanged )
                    callback += () => NotifyChanged ();

                var command = new SetPropertyCommand<T> (new Ref<T> (setter, getter), newValue, callback, pathName, propertyName);
                undoRedoManager.Do (command as ICommand);
            }
        }
    }
}
