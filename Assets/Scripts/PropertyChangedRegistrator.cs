using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tilify.Commands;

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
            if ( undoRedoRegister is null )
                throw new ArgumentNullException (nameof (undoRedoRegister) + " is null");
            this.undoRedoRegister = undoRedoRegister;
        }

        protected void NotifyNeedUpdate ()
            => OnNeedUpdate?.Invoke (this);

        protected void NotifyPropertyChanged (string propertyName)
            => OnPropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));

        /// <summary>
        /// Validates and sets the property to a new value. Creates and registers a new SetPropertyCommand in UndoRedoRegister.
        /// </summary>
        protected void SetProperty<T> (Action<T> setter, Func<T> getter, T newValue,
                                       bool needUpdateAfterFieldChange = false,
                                       Func<T, T> validator = null,
                                       [CallerFilePath] string pathName = "",
                                       [CallerMemberName] string propertyName = "")
        {
            if (validator != null)
                newValue = validator (newValue);
           
            if ( !EqualityComparer<T>.Default.Equals (getter(), newValue) )
            {
                Action callback = () => NotifyPropertyChanged(propertyName);

                if (needUpdateAfterFieldChange)
                    callback += () => NotifyNeedUpdate();

                var command = new SetPropertyCommand<T> (new Ref<T>(setter, getter), newValue, callback, pathName, propertyName);
                undoRedoRegister.Do (command as ICommand);
            }
        }
    }
}
