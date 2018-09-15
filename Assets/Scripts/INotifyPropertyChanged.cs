using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilify
{
    public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs args);
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler OnPropertyChanged;
    }

    public class PropertyChangedEventArgs : EventArgs
    {
        public readonly string propertyName;

        public PropertyChangedEventArgs(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}
