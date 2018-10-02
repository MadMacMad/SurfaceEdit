using System.Collections;
using System.Collections.Generic;
using SurfaceEdit.Brushes;
using UnityEngine;

namespace SurfaceEdit
{
    public class Bootstrap : MonoBehaviour
    {
        private UndoRedoManager undoRedoManager;
        private InputManager inputManager;

        private void Start ()
        {
            undoRedoManager = new UndoRedoManager ();

            var inputManager = new InputManager ();

            var chain = new InputTriggerConflictChain (
                new InputTriggerKeyCombination (undoRedoManager.Undo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F),

                new InputTriggerKeyCombination (undoRedoManager.Redo)
                    .WhenKeyPress (KeyCode.LeftControl)
                    .WhenKeyPress (KeyCode.LeftShift)
                    .WhenAnyKeyDown (KeyCode.Z, KeyCode.F));

            inputManager.AddTrigger (chain);
            
            var paintingManager = new PaintingManager ();
        }
    }
}