using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tilify.Commands;

namespace Tilify
{
    public abstract class PropertyChangedRegistrator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly UndoRedoRegister undoRedoRegister;

        public PropertyChangedRegistrator(UndoRedoRegister undoRedoRegister)
        {
            if ( undoRedoRegister is null )
                throw new ArgumentNullException (nameof (undoRedoRegister) + " is null");
            this.undoRedoRegister = undoRedoRegister;
        }

        protected void SetProperty<T> (Action<T> setter, Func<T> getter, T newValue,
                                       Func<T, T> validator = null,
                                       [CallerFilePath] string pathName = "",
                                       [CallerMemberName] string propertyName = "")
        {
            if (validator != null)
                newValue = validator (newValue);
           
            if ( !EqualityComparer<T>.Default.Equals (getter(), newValue) )
            {
                var command = new SetPropertyCommand<T> (new Ref<T>(setter, getter), newValue,
                                                        () => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName)),
                                                        pathName, propertyName);
                undoRedoRegister.Do (command as ICommand);
            }
        }
    }
}
