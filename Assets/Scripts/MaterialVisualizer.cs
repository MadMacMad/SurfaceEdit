using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.Commands;
using UnityEngine;

namespace Tilify
{
    public class MaterialVisualizer : ObjectChangedRegistrator, IDisposable
    {
        private static Dictionary<TextureChannel, string> channelPropertyPair = new Dictionary<TextureChannel, string> ()
        {
            { TextureChannel.Albedo, "_MainTex" },
            { TextureChannel.Normal, "_Normal" },
            { TextureChannel.Roughness, "_Roughness" },
            { TextureChannel.Metallic, "_Metallic" },
            { TextureChannel.HeightDisplacement, "_Displacement" }
        };

        public float DisplacementIntensity
        {
            get => displacementIntensity;
            set => SetProperty (v => displacementIntensity = v, () => displacementIntensity, value, true, v => Mathf.Clamp01 (v));
        }
        private float displacementIntensity;

        public float TesselationMultiplier
        {
            get => tesselationMultiplier;
            set => SetProperty (v => tesselationMultiplier = v, () => tesselationMultiplier, value, true, v => Mathf.Clamp (v, 1, 64));
        }
        private float tesselationMultiplier;

        private GameObject go;
        private Renderer goRenderer;

        public MaterialVisualizer (UndoRedoRegister undoRedoRegister, Surface textureStack,
                                  float displacementIntensity, float tesselationMultiplier, bool invertNormal = false) : base (undoRedoRegister)
        {
            go = new GameObject ("Material Visualzation Plane");

            go.transform.Rotate (90, 0, 0);

            go.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildPlane (textureStack.WorldSize, new Vector2Int(64, 64)).ConvertToMesh ();

            goRenderer = go.AddComponent<MeshRenderer> ();
            goRenderer.material.shader = Shader.Find ("Custom/TesselationDisplacementShader");

            foreach ( var pair in textureStack.Textures )
                if ( channelPropertyPair.ContainsKey (pair.Key) )
                    goRenderer.material.SetTexture (channelPropertyPair[pair.Key], pair.Value);

            goRenderer.material.SetFloat ("_InvertNormal", ( invertNormal ) ? 1 : 0);

            this.displacementIntensity = Mathf.Clamp01 (displacementIntensity);
            this.tesselationMultiplier = Mathf.Clamp (tesselationMultiplier, 1, 64);

            goRenderer.material.SetFloat ("_TesselationMultiplier", tesselationMultiplier);
            goRenderer.material.SetFloat ("_DisplacementIntensity", displacementIntensity);
        }

        public void Dispose ()
        {
            GameObject.Destroy (go);
        }
    }
}