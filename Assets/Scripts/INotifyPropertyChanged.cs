using System;

namespace SurfaceEdit
{
    public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs args);
    public delegate void NeedUpdateEventHandler (object sender, EventArgs eventArgs);

    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
    public interface INotifyNeedUpdate
    {
        event NeedUpdateEventHandler NeedUpdate;
    }

    public class PropertyChangedEventArgs : EventArgs
    {
        public readonly string propertyName;

        public PropertyChangedEventArgs(string propertyName = "")
        {
            this.propertyName = propertyName;
        }
    }
}
