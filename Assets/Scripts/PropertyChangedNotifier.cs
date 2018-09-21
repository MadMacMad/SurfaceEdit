using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SurfaceEdit
{
    public delegate void NeedUpdateEventHandler (object sender);

    public abstract class PropertyChangedNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NeedUpdateEventHandler NeedUpdate;

        protected void NotifyNeedUpdate ()
            => NeedUpdate?.Invoke (this);

        protected void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));

        /// <summary>
        /// Validates and modifies the new value with validator.
        /// If the validation is not successful, nothing happens.
        /// Otherwise, if the new value is not equal to the current value:
        /// 1. Sets the property to a new value and executes NotifyPropertyChanged.
        /// 2. If needUpdateAfterFieldChange is set to true, NotifyNeedUpdate will be executed.
        /// </summary>
        protected void SetPropertyValidate<T> (ref T property, T newValue, bool needUpdateAfterFieldChange = false, Func<T, Tuple<bool, T>> validator = null, [CallerMemberName] string propertyName = "")
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
                if ( needUpdateAfterFieldChange )
                    NotifyNeedUpdate ();
            }
        }

        /// <summary>
        /// Modifies the new value with modifier.
        /// If the new value is not equal to the current value:
        /// 1. Sets the property to a new value and executes NotifyPropertyChanged.
        /// 2. If needUpdateAfterFieldChange is set to true, NotifyNeedUpdate will be executed.
        /// </summary>
        protected void SetProperty<T> (ref T property, T newValue, bool needUpdateAfterFieldChange = false, Func<T, T> modifier = null, [CallerMemberName] string propertyName = "")
        {
            if ( modifier != null )
                newValue = modifier (newValue);

            if ( !EqualityComparer<T>.Default.Equals (property, newValue) )
            {
                property = newValue;
                NotifyPropertyChanged (propertyName);
                if ( needUpdateAfterFieldChange )
                    NotifyNeedUpdate ();
            }
        }
    }
}
