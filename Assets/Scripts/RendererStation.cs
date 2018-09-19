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
        // TODO: Cache system

        private static readonly float stackedObjectsZOffset = -.00001f;

        private Camera camera;
        private Scene scene;
        private GameObject rootObject;
        private GameObject texturePlane;
        private Renderer texturePlaneRenderer;

        private string id;
        private int stationLayerID;

        private int stackedObjectsCount;

        public RendererStation (int stationLayerID)
        {
            this.stationLayerID = stationLayerID;
            
            Setup ();
        }

        private void Setup ()
        {
            stackedObjectsCount = 1;
            id = Guid.NewGuid ().ToString ();
            scene = SceneManager.CreateScene (nameof (RendererStation) + " Scene with ID = " + id);
            rootObject = Utils.CreateNewGameObjectAtSpecificScene ("Root", scene, stationLayerID);

            camera = Utils.CreateNewGameObjectAtSpecificScene ("Camera", scene, stationLayerID).AddComponent<Camera> ();
            camera.transform.position = new Vector3 (.5f, .5f);
            camera.backgroundColor = new Color (1, 1, 1, 0);
            camera.orthographic = true;
            camera.orthographicSize = .5f;
            camera.farClipPlane = 10;
            camera.nearClipPlane = -100000000;
            camera.enabled = false;
            camera.cullingMask = LayerMask.GetMask(LayerMask.LayerToName(stationLayerID));

            texturePlane = Utils.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, stationLayerID, rootObject);
            texturePlane.transform.position = new Vector3 (0, 0, .2f);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildQuad (Vector2.one).ConvertToMesh ();

            texturePlaneRenderer = texturePlane.AddComponent<MeshRenderer> ();
            texturePlaneRenderer.material.shader = Shader.Find ("Unlit/Transparent");

            rootObject.SetActive (false);
        }

        public void UseIt (GameObject go)
        {
            Assert.ArgumentNotNull (go, nameof (go));

            go.transform.parent = rootObject.transform; // GameObject will automatically move to our scene
            var position = go.transform.position;
            position.z = stackedObjectsZOffset * stackedObjectsCount++;
            go.transform.position = position;
            go.layer = stationLayerID;
        }

        public void Render(RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));
            
            texturePlaneRenderer.material.mainTexture = texture;
            camera.targetTexture = texture;

            rootObject.SetActive (true);
            camera.Render ();
            rootObject.SetActive (false);

            texturePlaneRenderer.material.mainTexture = null;
            camera.targetTexture = null;
        }

        public void Reset ()
        {
            SceneManager.UnloadSceneAsync (scene);
            Setup ();
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync (scene);
        }
    }
}
