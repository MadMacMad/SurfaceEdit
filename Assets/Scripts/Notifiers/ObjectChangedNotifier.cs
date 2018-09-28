using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SurfaceEdit
{

    public abstract class ObjectChangedNotifier : INotifyPropertyChanged, INotifyObjectChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ObjectChangedEventHandler Changed;

        protected void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
        { 
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
            
        }
        protected void NotifyChanged ()
            => Changed?.Invoke (this, null);

        /// <summary>
        /// Validates and modifies the new value with validator.
        /// If the validation is not successful, nothing happens.
        /// Otherwise, if the new value is not equal to the current value:
        /// 1. Sets the property to a new value and executes NotifyPropertyChanged.
        /// 2. If notifyObjectChanged is set to true, NotifyObjectChanged will be executed.
        /// </summary>
        protected void SetPropertyValidate<T> (ref T property, T newValue, bool notifyObjectChanged = false, Func<T, Tuple<bool, T>> validator = null, [CallerMemberName] string propertyName = "")
        {
            if ( validator != null )
            {
                var result = validator (newValue);
                if ( !result.Item1 )
                    return;
                newValue = result.Item2;
            }

            if ( !EqualityComparer<T>.Default.Equals (property, newValue) )
            {
                property = newValue;
                NotifyPropertyChanged (propertyName);
                if ( notifyObjectChanged )
                    NotifyChanged ();
            }
        }

        /// <summary>
        /// Modifies the new value with modifier.
        /// If the new value is not equal to the current value:
        /// 1. Sets the property to a new value and executes NotifyPropertyChanged.
        /// 2. If notifyObjectChanged is set to true, NotifyObjectChanged will be executed.
        /// </summary>
        protected void SetProperty<T> (ref T property, T newValue, bool notifyObjectChanged = false, Func<T, T> modifier = null, [CallerMemberName] string propertyName = "")
        {
            if ( modifier != null )
                newValue = modifier (newValue);

            if ( !EqualityComparer<T>.Default.Equals (property, newValue) )
            {
                property = newValue;
                NotifyPropertyChanged (propertyName);
                if ( notifyObjectChanged )
                    NotifyChanged ();
            }
        }
    }
}
