using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public class Bootstrap : MonoBehaviour
    {
        private UndoRedoRegister undoRedoRegister;

        private void Start ()
        {
            undoRedoRegister = new UndoRedoRegister ();

            var inputManager = new InputManager ();

            var chain = new InputTriggerConflictChain (
                new InputTriggerKeyCombination (undoRedoRegister.Undo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F),

                new InputTriggerKeyCombination (undoRedoRegister.Redo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenKeyPress (KeyCode.LeftShift)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F));

            inputManager.AddTrigger (chain);
        }
    }
}