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
            { TextureChannel.Height, "_Displacement" }
        };

        public float DisplacementIntensity
        {
            get => displacementIntensity;
            set => SetPropertyAndRegisterUndoRedo (v => displacementIntensity = v, () => displacementIntensity, value, true, v => Mathf.Clamp01 (v));
        }
        private float displacementIntensity;

        public float TesselationMultiplier
        {
            get => tesselationMultiplier;
            set => SetPropertyAndRegisterUndoRedo (v => tesselationMultiplier = v, () => tesselationMultiplier, value, true, v => Mathf.Clamp (v, 1, 64));
        }
        private float tesselationMultiplier;

        public bool InvertNormal
        {
            get => invertNormal;
            set => SetPropertyAndRegisterUndoRedo (v => invertNormal = v, () => invertNormal, value, true);
        }
        private bool invertNormal;

        private GameObject go;
        private Renderer goRenderer;

        public MaterialVisualizer (UndoRedoRegister undoRedoRegister, Surface surface, Vector2 worldSize,
                                  float displacementIntensity, float tesselationMultiplier, bool invertNormal = false) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            go = new GameObject ("Material Visualzation Plane");

            go.transform.Rotate (90, 0, 0);

            go.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildPlane (worldSize, new Vector2Int(64, 64)).ConvertToMesh ();

            goRenderer = go.AddComponent<MeshRenderer> ();
            goRenderer.material.shader = Shader.Find ("Tilify/Advanced/TesselationDisplacementShader");

            foreach ( var pair in surface.Textures )
                if ( channelPropertyPair.ContainsKey (pair.Key) )
                    goRenderer.material.SetTexture (channelPropertyPair[pair.Key], pair.Value);

            this.displacementIntensity = Mathf.Clamp01 (displacementIntensity);
            this.tesselationMultiplier = Mathf.Clamp (tesselationMultiplier, 1, 64);
            this.invertNormal = invertNormal;

            Update (this);
            OnNeedUpdate += Update;
        }

        private void Update(object sender)
        {
            goRenderer.material.SetFloat ("_TesselationMultiplier", tesselationMultiplier);
            goRenderer.material.SetFloat ("_DisplacementIntensity", displacementIntensity);
            goRenderer.material.SetFloat ("_InvertNormal", ( invertNormal ) ? 1 : 0);
        }

        public void Dispose ()
        {
            GameObject.Destroy (go);
        }
    }
}