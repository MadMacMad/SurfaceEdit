﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurfaceEdit
{
    public static class Utils
    {
        public static RenderTexture CreateRenderTexture (int width, int height)
        {
            //Debug.Log ("Render Texture Allocated (Width: " + width + ", Height: " + height + ")");
            RenderTexture renderTexture = new RenderTexture (width, height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            renderTexture.Create ();
            return renderTexture;
        }

        public static RenderTexture CreateRenderTexture (int size)
        {
            return CreateRenderTexture (size, size);
        }

        public static RenderTexture CreateRenderTexture (Vector2Int size)
        {
            return CreateRenderTexture (size.x, size.y);
        }

        public static Object InstantiateAtSpecificScene (Object original, Vector3 position, Quaternion rotation, Scene scene, int layerID, GameObject parent = null)
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

        public static GameObject CreateNewGameObjectAtSpecificScene(string name, Scene scene, int layerID, GameObject parent = null)
        {
            var currentScene = SceneManager.GetActiveScene ();
            SceneManager.SetActiveScene (scene);
            var obj = new GameObject (name)
            {
                layer = layerID
            };

            if (parent != null)
                obj.transform.parent = parent.transform;

            SceneManager.SetActiveScene (currentScene);
            return obj;
        }
    }
}
