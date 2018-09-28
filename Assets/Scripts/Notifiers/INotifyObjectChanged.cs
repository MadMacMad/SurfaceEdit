using System;

namespace SurfaceEdit
{
    public delegate void ObjectChangedEventHandler (object sender, EventArgs eventArgs);

    public interface INotifyObjectChanged
    {
        event ObjectChangedEventHandler Changed;
    }
}
