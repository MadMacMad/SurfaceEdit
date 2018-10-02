using System;
using System.Collections.Generic;

using SurfaceEdit.Commands;

namespace SurfaceEdit
{
    public class UndoRedoRegister
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoSrack = new Stack<ICommand>();
        
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
            Log (ActionType.DO, command);
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
                Log (ActionType.REDO, command);
                undoStack.Push (command);
            }
        }
        public void Undo()
        {
            if ( undoStack.Count > 0)
            {
                var command = undoStack.Pop ();
                command.Undo ();
                Log (ActionType.UNDO, command);
                redoSrack.Push (command);
            }
        }

        private void Log(ActionType actionType, ICommand command)
        {
            Logger.Log ($"{DateTime.Now.ToShortTimeString ()} {actionType.ToString()}({command.GetType ().Name}):\n{command.ToString ()}");
        }
        
        private enum ActionType
        {
            DO,
            REDO,
            UNDO
        }
    }
}
