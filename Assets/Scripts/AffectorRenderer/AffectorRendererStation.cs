using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify.AffectorRenderer
{
    public class AffectorRendererStation : IDisposable
    {
        public string ID { get; private set; }
        public Camera Camera { get; private set; }

        private RenderTexture targetTexture;
        private RenderTexture baseTexture;
        private Vector2 textureWorldSize;

        private Scene scene;
        private GameObject rootObject;
        private GameObject texturePlane;

        private int renderIgnoreLayerID;

        public AffectorRendererStation(int renderIgnoreLayerID, RenderTexture baseTexture, RenderTexture targetTexture, Vector2 textureWorldSize)
        {
            Assert.ArgumentNotNull (targetTexture, nameof (targetTexture));
            Assert.ArgumentNotNull (baseTexture, nameof (baseTexture));

            this.targetTexture = targetTexture;
            this.baseTexture = baseTexture;
            this.textureWorldSize = textureWorldSize = TextureHelper.Instance.ClampWorldSize (textureWorldSize);
            this.renderIgnoreLayerID = renderIgnoreLayerID;
            ID = Guid.NewGuid ().ToString ();
            
            scene = SceneManager.CreateScene (ID);
            Setup ();
        }

        private void Setup()
        {
            rootObject = Utils.CreateNewGameObjectAtSpecificScene ("Parent", scene, renderIgnoreLayerID);

            Camera = Utils.CreateNewGameObjectAtSpecificScene(ID + " Camera", scene, renderIgnoreLayerID, rootObject).AddComponent<Camera> ();
            Camera.transform.Translate (textureWorldSize.x / 2f, textureWorldSize.y / 2f, 0);
            Camera.backgroundColor = new Color (1, 1, 1, 0);
            Camera.orthographic = true;
            Camera.farClipPlane = 1;
            Camera.enabled = false;
            Camera.targetTexture = targetTexture;

            texturePlane = Utils.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, renderIgnoreLayerID, rootObject);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildQuad (textureWorldSize).ConvertToMesh ();
            var renderer = texturePlane.AddComponent<MeshRenderer> ();
            renderer.material.shader = Shader.Find ("Unlit/Transparent");
            renderer.material.mainTexture = baseTexture;
        }

        public void Render()
        {

        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync (scene);
        }
    }
}
