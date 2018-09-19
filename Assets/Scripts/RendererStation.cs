using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify
{
    public class RendererStation : IDisposable
    {
        private static readonly float stackedObjectsZOffset = -.00001f;

        private Camera camera;
        private Scene scene;
        private GameObject rootObject;
        private GameObject texturePlane;
        private Renderer texturePlaneRenderer;

        private string id;
        private int stationLayerID;

        private int stackedObjectsCount = 1;

        public RendererStation (int stationLayerID)
        {
            this.stationLayerID = stationLayerID;
            id = Guid.NewGuid ().ToString ();

            scene = SceneManager.CreateScene (nameof(RendererStation) + " Scene with ID = " + id);
            Setup ();
        }

        private void Setup ()
        {
            rootObject = Utils.CreateNewGameObjectAtSpecificScene ("Root", scene, stationLayerID);

            camera = Utils.CreateNewGameObjectAtSpecificScene ("Camera", scene, stationLayerID, rootObject).AddComponent<Camera> ();
            camera.backgroundColor = new Color (1, 1, 1, 0);
            camera.orthographic = true;
            camera.orthographicSize = .5f;
            camera.farClipPlane = 1;
            camera.nearClipPlane = -float.MaxValue;
            camera.enabled = false;
            camera.cullingMask = LayerMask.GetMask(LayerMask.LayerToName(stationLayerID));

            texturePlane = Utils.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, stationLayerID, rootObject);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildQuad (Vector2.one).ConvertToMesh ();

            texturePlaneRenderer = texturePlane.AddComponent<MeshRenderer> ();
            texturePlaneRenderer.material.shader = Shader.Find ("Tilify/Unlit/Transparent");

            rootObject.SetActive (false);
        }

        public void UseIt (GameObject go)
        {
            Assert.ArgumentNotNull (go, nameof (go));

            go.transform.parent = rootObject.transform; // GameObject will automatically move to our scene
            var position = go.transform.position;
            position.z = stackedObjectsZOffset * stackedObjectsCount++;
            go.transform.position = position;
        }

        public void StopUseIt (GameObject go)
        {
            Assert.ArgumentNotNull (go, nameof (go));

            if ( go.scene != scene )
                Debug.Log ("GameObject is already not in the scene used by the station");
            else
            {
                go.transform.parent = null; // Move GameObject to default scene
                go.SetActive (true);
            }
        }

        public void Render(RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            texturePlaneRenderer.material.mainTexture = texture;
            camera.targetTexture = texture;

            rootObject.SetActive (true);
            camera.Render ();
            rootObject.SetActive (false);
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync (scene);
        }
    }
}
