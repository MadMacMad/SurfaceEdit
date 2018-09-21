using System;

namespace SurfaceEdit
{
    public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs args);
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
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
