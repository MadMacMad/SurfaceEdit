using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurfaceEdit
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
            rootObject = GameObjectUtility.CreateNewGameObjectAtSpecificScene ("Root", scene, stationLayerID);

            camera = GameObjectUtility.CreateNewGameObjectAtSpecificScene ("Camera", scene, stationLayerID).AddComponent<Camera> ();
            camera.transform.position = new Vector3 (.5f, .5f);
            camera.backgroundColor = new Color (1, 1, 1, 0);
            camera.orthographic = true;
            camera.orthographicSize = .5f;
            camera.farClipPlane = 10;
            camera.nearClipPlane = -100000000;
            camera.enabled = false;
            camera.cullingMask = LayerMask.GetMask(LayerMask.LayerToName(stationLayerID));
            camera.useOcclusionCulling = false;

            texturePlane = GameObjectUtility.CreateNewGameObjectAtSpecificScene ("Texture Plane", scene, stationLayerID, rootObject);
            texturePlane.transform.position = new Vector3 (0, 0, .2f);
            texturePlane.AddComponent<MeshFilter> ().mesh = MeshUtility.BuildQuad (Vector2.one).ConvertToMesh ();

            texturePlaneRenderer = texturePlane.AddComponent<MeshRenderer> ();
            texturePlaneRenderer.material.shader = Shader.Find ("Unlit/Transparent");

            rootObject.SetActive (false);
        }

        public void UseIt (GameObject go, float width) => UseIt_Internal (go, width, false);
        public void UseItTemporary (GameObject go, float width) => UseIt_Internal (go, width, true);

        private void UseIt_Internal(GameObject go, float width, bool isTemporary)
        {
            Assert.ArgumentNotNull (go, nameof (go));
            width = Mathf.Clamp (width, 0, 10000);

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

        public void Render(RenderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            var textureSize = (Vector2)texture.GetVectorSize ();

            var originNormalized = pixelPosition / textureSize;
            var sizeNormalized = pixelSize / textureSize;

            var rect = new Rect (originNormalized, sizeNormalized);

            texturePlaneRenderer.material.mainTexture = texture;
            camera.targetTexture = texture;

            camera.rect = rect;
            camera.transform.position = rect.center;
            camera.orthographicSize = rect.height / 2f;

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
