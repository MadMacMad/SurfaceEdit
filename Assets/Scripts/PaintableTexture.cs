using System;
using Tilify.Brushes;
using Tilify.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify
{
    /// <summary>
    /// Just for reference. In near future will be completely removed
    /// </summary>
    [Obsolete]
    public class PaintableTexture : IDisposable
    {
        public RenderTexture ResultTexture { get; private set; }

        public Vector2 TextureWorldSize
        {
            get { return textureWorldSize; }
            set
            {
                if ( value.x <= 0 )
                    value.x = 1;
                if ( value.y <= 0 )
                    value.y = 1;

                if ( textureWorldSize != value)
                {
                    textureWorldSize = value;
                    Update ();
                }
            }
        }
        private Vector2 textureWorldSize;

        public RenderTexture baseTexture;

        private Scene scene;
        private Camera camera;
        private GameObject texturePlane;

        private UndoRedoRegister undoRedoRegister;

        private int brushCount = 1;

        private static readonly float brushZOffset = -.0001f;

        private int layerID;


        public PaintableTexture (Vector2 textureWorldSize, RenderTexture baseTexture, UndoRedoRegister undoRedoRegister, int layerID)
        {
            this.baseTexture = baseTexture;
            ResultTexture = Utils.CreateAndAllocateRenderTexture (baseTexture.width, baseTexture.height);

            this.textureWorldSize = textureWorldSize;

            this.undoRedoRegister = undoRedoRegister;

            this.layerID = layerID;

            SetupScene ();
            Update ();
        }

        public void Paint (Brush brush, Vector2 percentagePosition)
        {
            var paintEntry = new BrushPaintEntry (brushCount, textureWorldSize, brush, percentagePosition, brushCount++ * brushZOffset, scene, layerID);
            var command = new PaintCommand (this, paintEntry);
            undoRedoRegister.Do (command);
        }
        public void PaintWrapped(Brush brush, Vector2 percentagePosition)
        {

        }

        public void Dispose ()
        {
            ResultTexture.Release ();
            SceneManager.UnloadSceneAsync (scene);
        }

        private void SetupScene ()
        {
            var id = Guid.NewGuid ().ToString ();
            scene = SceneManager.CreateScene (id);

            camera = Utils.CreateNewGameObjectAtSpecificScene ("Camera", scene, layerID).AddComponent<Camera> ();
            camera.transform.Translate(textureWorldSize.x / 2f, textureWorldSize.y / 2f, 0);
            camera.backgroundColor = new Color (1, 1, 1, 0);
            camera.orthographic = true;
            camera.farClipPlane = 1;
            camera.enabled = false;
            camera.targetTexture = ResultTexture;

            texturePlane = Utils.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, layerID);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildQuad (textureWorldSize).ConvertToMesh();
            var renderer = texturePlane.AddComponent<MeshRenderer> ();
            renderer.material.shader = Shader.Find ("Unlit/Transparent");
            renderer.material.mainTexture = baseTexture;
        }
        
        private void Update()
        {
            camera.orthographicSize = textureWorldSize.y / 2;

            Render ();
        }

        private void Render ()
        {
            camera.nearClipPlane = ( brushCount + 100) * brushZOffset;
            camera.Render ();
        }

        private class PaintCommand : ICommand
        {
            private PaintableTexture paintableTexture;
            private BrushPaintEntry paintEntry;

            public PaintCommand(PaintableTexture paintableTexture, BrushPaintEntry paintEntry)
            {
                this.paintableTexture = paintableTexture;
                this.paintEntry = paintEntry;
            }

            public void Dispose ()
            {
                paintEntry.Dispose ();
            }

            public void Do ()
            {
                paintEntry.Show ();
                paintableTexture.Render ();
            }

            public void Undo ()
            {
                paintEntry.Hide ();
                paintableTexture.Render ();
            }
        }
    }
    public class BrushPaintEntry : IDisposable
    {
        public Vector2 TextureWorldSize
        {
            get { return textureWorldSize; }
            set
            {
                if ( value.x <= 0 )
                    value.x = 1;
                if ( value.y <= 0 )
                    value.y = 1;

                textureWorldSize = value;
            }
        }
        private Vector2 textureWorldSize;

        private Brush brush;
        private Vector2 percentagePosition;
        private float zWorldPosition;
        private GameObject go;
        private int entryID;
        private int layerID;
        private Scene scene;
        
        public BrushPaintEntry (int entryID, Vector2 textureWorldSize, Brush brush, Vector2 percentagePosition, float zWorldPosition, Scene scene, int layerID)
        {
            this.brush = brush;
            this.percentagePosition = percentagePosition;
            this.zWorldPosition = zWorldPosition;
            this.textureWorldSize = textureWorldSize;
            this.entryID = entryID;
            this.layerID = layerID;
            this.scene = scene;
        }

        public void Update ()
        {
            if ( go != null )
                GameObject.Destroy (go);

            //go = brush.CreateGO (entryID.ToString (), percentagePosition, zWorldPosition, textureWorldSize, scene, layerID);
        }
        
        public void Show ()
        {
            if ( go == null )
                Update ();

            go.SetActive (true);
        }
        
        public void Hide ()
        {
            go?.SetActive (false);
        }

        public void Dispose ()
        {
            GameObject.Destroy (go);
        }
    }
}
