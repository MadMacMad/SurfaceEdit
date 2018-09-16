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
        private static readonly float stackedObjectsZOffset = -.00001f;

        public string ID { get; private set; }
        public RenderTexture TargetTexture { get; private set; }
        public Camera Camera { get; private set; }

        private RenderTexture baseTexture;
        private Vector2 textureWorldSize;

        private Scene scene;
        private GameObject rootObject;
        private GameObject texturePlane;

        private int stationLayerID;

        public AffectorRendererStation (int stationLayerID, RenderTexture baseTexture, RenderTexture targetTexture, Vector2 textureWorldSize)
        {
            Assert.ArgumentNotNull (targetTexture, nameof (targetTexture));
            Assert.ArgumentNotNull (baseTexture, nameof (baseTexture));

            TargetTexture = targetTexture;
            this.baseTexture = baseTexture;
            this.textureWorldSize = textureWorldSize = TextureHelper.Instance.ClampWorldSize (textureWorldSize);
            this.stationLayerID = stationLayerID;
            ID = Guid.NewGuid ().ToString ();

            scene = SceneManager.CreateScene (nameof(AffectorRendererStation) + " Scene with ID = " + ID);
            Setup ();
        }

        private void Setup ()
        {
            rootObject = Utils.CreateNewGameObjectAtSpecificScene ("Root", scene, stationLayerID);

            Camera = Utils.CreateNewGameObjectAtSpecificScene ("Camera", scene, stationLayerID, rootObject).AddComponent<Camera> ();
            Camera.transform.Translate (textureWorldSize.x / 2f, textureWorldSize.y / 2f, 0);
            Camera.backgroundColor = new Color (1, 1, 1, 0);
            Camera.orthographic = true;
            Camera.orthographicSize = textureWorldSize.y / 2;
            Camera.farClipPlane = 1;
            Camera.nearClipPlane = -float.MaxValue;
            Camera.enabled = false;
            Camera.targetTexture = TargetTexture;
            Camera.cullingMask = LayerMask.GetMask(LayerMask.LayerToName(stationLayerID));

            texturePlane = Utils.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, stationLayerID, rootObject);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildQuad (textureWorldSize).ConvertToMesh ();
            var renderer = texturePlane.AddComponent<MeshRenderer> ();
            renderer.material.shader = Shader.Find ("Tilify/Unlit/Transparent");
            renderer.material.mainTexture = baseTexture;

            rootObject.SetActive (false);
        }

        public void UseIt (GameObject go)
        {
            go.transform.parent = rootObject.transform; // GameObject will automatically move to our scene
        }

        public void StopUseIt (GameObject go)
        {
            if ( go.scene != scene )
                Debug.Log ("GameObject is already not in the scene used by the station");
            else
            {
                go.transform.parent = null; // Move GameObject to default scene
                go.SetActive (true);
            }
        }

        public void Render()
        {
            rootObject.SetActive (true);
            Camera.Render ();
            rootObject.SetActive (false);
        }

        public void Dispose()
        {
            TargetTexture.Release ();
            SceneManager.UnloadSceneAsync (scene);
        }
    }
}
