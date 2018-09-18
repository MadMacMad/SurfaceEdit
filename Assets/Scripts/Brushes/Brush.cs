using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify.Brushes
{
    public abstract class Brush : IDisposable
    {
        public Vector2 PercentageSize
        {
            get => percentageSize;
            set
            {
                value.Clamp01 ();
                percentageSize = value;
            }
        }
        private Vector2 percentageSize;

        public float Intervals
        {
            get => intervals;
            set
            {
                value = Mathf.Clamp (value, .01f, 10f);
                intervals = value;
            }
        }
        private float intervals;

        public RenderTexture BrushStamp { get; protected set; }

        protected Brush (Vector2 percentageSize, float intervals)
        {
            PercentageSize = percentageSize;
            Intervals = intervals;
        }

        public GameObject CreateGO (string name, Vector2 percentagePosition, float zWorldPosition, Vector2 textureWorldSize, Scene scene, int layerID)
        {
            percentagePosition.Clamp01 ();

            var go = Utils.CreateNewGameObjectAtSpecificScene (name, scene, layerID);

            Vector3 worldPosition = (percentagePosition - PercentageSize / 2) * textureWorldSize;
            worldPosition.z = zWorldPosition;
            go.transform.position = worldPosition;

            var mesh = MeshBuilder.BuildQuad (PercentageSize * textureWorldSize).ConvertToMesh();
            go.AddComponent<MeshFilter> ().mesh = mesh;

            var renderer = go.AddComponent<MeshRenderer> ();
            renderer.material.shader = Shader.Find ("Unlit/Transparent");
            renderer.material.mainTexture = BrushStamp;

            return go;
        }

        public virtual void Dispose ()
        {
            BrushStamp.Release ();
        }
    }
}
