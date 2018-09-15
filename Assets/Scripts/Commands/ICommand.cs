using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tilify.Commands
{
    public interface ICommand : IDisposable
    {
        void Do();
        void Undo ();
    }
}
