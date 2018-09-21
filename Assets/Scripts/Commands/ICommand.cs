using System;

namespace SurfaceEdit.Commands
{
    public interface ICommand : IDisposable
    {
        void Do();
        void Undo ();
    }
}
