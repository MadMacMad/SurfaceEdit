using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Tilify.Commands;

namespace Tilify
{
    public class UndoRedoRegister : Singleton<UndoRedoRegister>
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoSrack = new Stack<ICommand>();
        
        public UndoRedoRegister() { }

        public void Reset()
        {
            foreach ( var c in redoSrack )
                c.Dispose ();
            foreach ( var c in undoStack )
                c.Dispose ();
            redoSrack.Clear ();
            undoStack.Clear ();
        }
        public void Do (ICommand command)
        {
            Assert.ArgumentNotNull (command, nameof (command));

            command.Do ();
            Logger.Log (DateTime.Now.ToShortTimeString() + " DO(" + command.GetType ().ToString () + ") " + command.ToString ());
            undoStack.Push (command);
            foreach ( var c in redoSrack )
                c.Dispose ();
            redoSrack.Clear ();
        }
        public void Redo()
        {
            if ( redoSrack.Count > 0)
            {
                var command = redoSrack.Pop ();
                command.Do ();
                Logger.Log (DateTime.Now.ToShortTimeString () + " REDO(" + command.GetType ().ToString () + ") " + command.ToString ());
                undoStack.Push (command);
            }
        }
        public void Undo()
        {
            if ( undoStack.Count > 0)
            {
                var command = undoStack.Pop ();
                command.Undo ();
                Logger.Log (DateTime.Now.ToShortTimeString () + " UNDO(" + command.GetType ().ToString () + ") " + command.ToString ());
                redoSrack.Push (command);
            }
        }
    }
}
