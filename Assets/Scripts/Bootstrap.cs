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
        private PaintingManager paintingManager;
        private SkyboxManager skyboxManager;

        private void Start ()
        {
            UnityMemorizer<Vector3>.Instance.Memorize ("mousePosition", () => Input.mousePosition);

            SetupUndoRedoManager ();
            SetupPaintingManager ();
            SetupInputManager ();
        }

        private void SetupInputManager ()
        {
            inputManager = new InputManager ();

            var undoTrigger = new KeyCombination ()
                .Ctrl ()
                .Key (KeyCode.Z)
                .AddTriggeredCallback(undoRedoManager.Undo);

            inputManager.AddTrigger (undoTrigger);

            var redoTrigger = new KeyCombination ()
                .Ctrl ()
                .Shift ()
                .Key (KeyCode.Z)
                .AddTriggeredCallback(undoRedoManager.Redo);

            inputManager.AddTrigger (redoTrigger);

            var skyboxRotationTrigger = new KeyCombination ()
                .Shift ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback(() =>
                {
                    var lastMousePosition = UnityMemorizer<Vector3>.Instance.GetValue ("mousePosition");
                    var mousePosition = Input.mousePosition;
                    var rotation = mousePosition.x - lastMousePosition.x;
                    skyboxManager.RotateSkyBoxIncremental (rotation);
                });

            inputManager.AddTrigger (skyboxRotationTrigger);

            var paintTrigger = new KeyCombination ()
                .Key (KeyCode.Mouse0, KeyTriggerType.Press)
                .AddTriggeredCallback(() =>
                {
                    bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out RaycastHit hit);

                    if ( isHit )
                    {
                        var point = new Vector2 (hit.point.x, hit.point.z);
                        paintingManager.PaintTriggered (point);
                    }
                })
                .AddNotTriggeredCallback(paintingManager.PaintNotTriggered);

            inputManager.AddTrigger (paintTrigger);
        }

        private void SetupUndoRedoManager()
        {
            undoRedoManager = new UndoRedoManager ();
        }
        private void SetupPaintingManager()
        {
            paintingManager = new PaintingManager ();
        }
        private void SetupSkyboxManager()
        {
            skyboxManager = SkyboxManager.Instance;
        }
    }
}