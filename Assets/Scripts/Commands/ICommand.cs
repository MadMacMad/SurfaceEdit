using System;

namespace Tilify.Commands
{
    public interface ICommand : IDisposable
    {
        void Do();
        void Undo ();
    }
}
