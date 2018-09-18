﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.Commands;
using UnityEngine;

namespace Tilify
{
    public class SurfaceVisualizer : ObjectChangedRegistrator, IDisposable
    {
        private static Dictionary<TextureChannel, string> channelPropertyPair = new Dictionary<TextureChannel, string> ()
        {
            { TextureChannel.Albedo, "_MainTex" },
            { TextureChannel.Normal, "_Normal" },
            { TextureChannel.Roughness, "_Roughness" },
            { TextureChannel.Metallic, "_Metallic" },
            { TextureChannel.Height, "_Displacement" }
        };
        private static Shader surfaceShader = Shader.Find ("Tilify/Advanced/TesselationDisplacementShader");
        private static Shader textureShader = Shader.Find ("Tilify/Advanced/TesselationDisplacementTextureShader");

        public float DisplacementIntensity
        {
            get => displacementIntensity;
            set => SetPropertyAndRegisterUndoRedo (v => displacementIntensity = v, () => displacementIntensity, value, true, v => Mathf.Clamp01 (v));
        }
        private float displacementIntensity = .1f;

        public float TesselationMultiplier
        {
            get => tesselationMultiplier;
            set => SetPropertyAndRegisterUndoRedo (v => tesselationMultiplier = v, () => tesselationMultiplier, value, true, v => Mathf.Clamp (v, 1, 64));
        }
        private float tesselationMultiplier = 5;

        public bool InvertNormal
        {
            get => invertNormal;
            set => SetPropertyAndRegisterUndoRedo (v => invertNormal = v, () => invertNormal, value, true);
        }
        private bool invertNormal = false;

        public SurfaceRenderMode RenderMode
        {
            get => renderMode;
            set => SetProperty (ref renderMode, value, true);
        }
        private SurfaceRenderMode renderMode;

        public TextureChannel RenderedChannel
        {
            get => renderedChannel;
            set => SetProperty (ref renderedChannel, value, renderMode == SurfaceRenderMode.Channel);
        }
        private TextureChannel renderedChannel = TextureChannel.Albedo;

        private GameObject go;
        private Renderer renderer;

        private Surface surface;

        public SurfaceVisualizer (UndoRedoRegister undoRedoRegister, Surface surface, Vector2 worldSize, SurfaceRenderMode renderMode = SurfaceRenderMode.Surface) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            this.renderMode = renderMode;
            this.surface = surface;

            go = new GameObject ("Surface Visualzation Plane");

            go.transform.Rotate (90, 0, 0);

            go.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildPlane (worldSize, new Vector2Int (64, 64)).ConvertToMesh ();

            renderer = go.AddComponent<MeshRenderer> ();

            Update (this);
            OnNeedUpdate += Update;
        }

        private void Update (object sender)
        {
            if ( renderMode == SurfaceRenderMode.Surface )
            {
                renderer.material.shader = surfaceShader;
                foreach ( var pair in surface.Textures )
                    if ( channelPropertyPair.ContainsKey (pair.Key) )
                        renderer.material.SetTexture (channelPropertyPair[pair.Key], pair.Value);
            }
            else
            {
                renderer.material.shader = textureShader;
                var texture = surface.Textures[renderedChannel];
                renderer.material.SetTexture ("_MainTex", texture);
            }

            renderer.material.SetFloat ("_TesselationMultiplier", tesselationMultiplier);
            renderer.material.SetFloat ("_DisplacementIntensity", displacementIntensity);
            renderer.material.SetFloat ("_InvertNormal", ( invertNormal ) ? 1 : 0);
        }

        public void Dispose ()
        {
            GameObject.Destroy (go);
        }

        public enum SurfaceRenderMode
        {
            Surface,
            Channel
        }
    }
}