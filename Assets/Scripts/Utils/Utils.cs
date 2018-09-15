using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify
{
    public static class Utils
    {
        public static RenderTexture CreateAndAllocateRenderTexture (int width, int height)
        {
            UnityEngine.RenderTexture renderTexture = new UnityEngine.RenderTexture (width, height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            renderTexture.Create ();
            return renderTexture;
        }

        public static RenderTexture CreateAndAllocateRenderTexture (int size)
        {
            return CreateAndAllocateRenderTexture (size, size);
        }

        public static RenderTexture CreateAndAllocateRenderTexture (Vector2Int size)
        {
            return CreateAndAllocateRenderTexture (size.x, size.y);
        }

        public static UnityEngine.Object InstantiateAtSpecificScene (UnityEngine.Object original, Vector3 position, Quaternion rotation, Scene scene, int layerID)
        {
            var currentScene = SceneManager.GetActiveScene ();
            SceneManager.SetActiveScene (scene);
            var obj = GameObject.Instantiate (original, position, rotation) as GameObject;
            obj.layer = layerID;
            SceneManager.SetActiveScene (currentScene);
            return obj;
        }

        public static GameObject CreateNewGameObjectAtSpecificScene(string name, Scene scene, int layerID)
        {
            var currentScene = SceneManager.GetActiveScene ();
            SceneManager.SetActiveScene (scene);
            var obj = new GameObject (name)
            {
                layer = layerID
            };
            SceneManager.SetActiveScene (currentScene);
            return obj;
        }
    }
}
