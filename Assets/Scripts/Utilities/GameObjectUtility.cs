using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurfaceEdit
{
    public static class GameObjectUtility
    {
        public static UnityEngine.Object InstantiateAtSpecificScene (UnityEngine.Object original, Vector3 position, Quaternion rotation, Scene scene, int layerID, GameObject parent = null)
        {
            Assert.ArgumentNotNull (original, nameof (original));

            var currentScene = SceneManager.GetActiveScene ();
            SceneManager.SetActiveScene (scene);

            var obj = GameObject.Instantiate (original, position, rotation) as GameObject;
            obj.layer = layerID;

            if ( parent != null )
                obj.transform.parent = parent.transform;

            SceneManager.SetActiveScene (currentScene);
            return obj;
        }

        public static GameObject CreateNewGameObjectAtSpecificScene (string name, Scene scene, int layerID, GameObject parent = null)
        {
            var currentScene = SceneManager.GetActiveScene ();
            SceneManager.SetActiveScene (scene);
            var obj = new GameObject (name)
            {
                layer = layerID
            };

            if ( parent != null )
                obj.transform.parent = parent.transform;

            SceneManager.SetActiveScene (currentScene);
            return obj;
        }
    }
}
