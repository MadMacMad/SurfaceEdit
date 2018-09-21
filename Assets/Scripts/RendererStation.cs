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
        private static readonly float distanceBetweenStackedObjects = -.00001f;
        private static readonly int defaultLayerID = LayerMask.NameToLayer("Default");

        private Camera camera;
        private Scene scene;
        private GameObject rootObject;
        private GameObject texturePlane;
        private Renderer texturePlaneRenderer;

        private List<GameObject> stackedObjects = new List<GameObject>();

        private string id;
        private int stationLayerID;

        private float stackedObjectsOffset;

        private GameObject temporaryObject;

        public RendererStation (int stationLayerID)
        {
            this.stationLayerID = stationLayerID;
            
            Setup ();
        }

        private void Setup ()
        {
            stackedObjectsOffset = 0;
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

        public void UseIt (GameObject go, float width) => UseIt_Internal (go, width, false);
        public void UseItTemporary (GameObject go, float width) => UseIt_Internal (go, width, true);

        private void UseIt_Internal(GameObject go, float width, bool isTemporary)
        {
            Assert.ArgumentNotNull (go, nameof (go));
            Assert.ArgumentTrue (width >= 0, nameof (width) + " is less then 0");

            if ( temporaryObject != null)
                GameObject.DestroyImmediate (temporaryObject);
            
            go.transform.parent = rootObject.transform; // GameObject will automatically move to our scene
            var position = go.transform.position;
            position.z = stackedObjectsOffset;
            go.transform.position = position;

            go.layer = stationLayerID;

            if ( isTemporary )
                temporaryObject = go;
            else
            {
                stackedObjectsOffset -= width + distanceBetweenStackedObjects;
                stackedObjects.Add (go);
            }
        }

        public void StopUseIt(GameObject go)
        {
            if ( stackedObjects.Contains (go))
            {
                go.layer = defaultLayerID;
                go.transform.parent = null; // Move gameObject to default scene
                stackedObjects.Remove (go);
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
