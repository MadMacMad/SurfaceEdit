using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tilify.Commands;
using UnityEngine;

namespace Tilify
{
    public delegate void NeedUpdateEventHandler (object sender);

    public abstract class ObjectChangedRegistrator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler OnPropertyChanged;
        public event NeedUpdateEventHandler OnNeedUpdate;

        protected readonly UndoRedoRegister undoRedoRegister;

        public ObjectChangedRegistrator(UndoRedoRegister undoRedoRegister)
        {
            Assert.ArgumentNotNull (undoRedoRegister, nameof (undoRedoRegister));
            this.undoRedoRegister = undoRedoRegister;
        }

        protected void NotifyNeedUpdate ()
            => OnNeedUpdate?.Invoke (this);

        protected void NotifyPropertyChanged (string propertyName = "")
            => OnPropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));

        /// <summary>
        /// Validates the new value. If the new value is not equal to the current value, sets the property to a new value and calls NotifyPropertyChanged 
        /// </summary>
        protected void SetProperty<T>(ref T property, T newValue, bool needUpdateAfterFieldChange = false, Func<T, T> validator = null, [CallerMemberName] string propertyName = "")
        {
            if ( validator != null )
                newValue = validator (newValue);

            if ( !EqualityComparer<T>.Default.Equals (property, newValue) )
            {
                property = newValue;
                NotifyPropertyChanged (propertyName);
                if ( needUpdateAfterFieldChange )
                    NotifyNeedUpdate ();
            }
        }
        /// <summary>
        /// Validates the new value. If the new value is not equal to the current value, it creates and registers a new undo/redo command.
        /// </summary>
        protected void SetPropertyAndRegisterUndoRedo<T> (Action<T> setter, Func<T> getter, T newValue,
                                       bool needUpdateAfterFieldChange = false,
                                       Func<T, T> validator = null,
                                       [CallerFilePath] string pathName = "",
                                       [CallerMemberName] string propertyName = "")
        {
            Assert.ArgumentNotNull (setter, nameof (setter));
            Assert.ArgumentNotNull (getter, nameof (getter));

            if (validator != null)
                newValue = validator (newValue);
           
            if ( !EqualityComparer<T>.Default.Equals (getter(), newValue) )
            {
                Action callback = () => NotifyPropertyChanged(propertyName);

                if (needUpdateAfterFieldChange)
                    callback += () => NotifyNeedUpdate ();

                var command = new SetPropertyCommand<T> (new Ref<T>(setter, getter), newValue, callback, pathName, propertyName);
                undoRedoRegister.Do (command as ICommand);
            }
        }
    }
}
