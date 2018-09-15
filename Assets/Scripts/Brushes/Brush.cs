﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tilify
{
    public abstract class Brush : IDisposable
    {
        public Vector2 PercentageSize { get; }

        public RenderTexture BrushStamp { get; protected set; }

        protected Brush (Vector2 percentageSize)
        {
            if ( percentageSize.x <= 0 )
                percentageSize.x = 1;
            if ( percentageSize.y <= 0 )
                percentageSize.y = 1;

            PercentageSize = percentageSize;
        }

        public GameObject CreateGO (string name, Vector2 percentagePosition, float zWorldPosition, Vector2 textureWorldSize, Scene scene, int layerID)
        {
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
