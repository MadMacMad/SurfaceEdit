using System;
using System.Collections.Generic;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public class SurfaceVisualizer : PropertyChangedRegistrator, IDisposable
    {
        private static Dictionary<Channel, string> channelPropertyPair = new Dictionary<Channel, string> ()
        {
            { Channel.Albedo, "_MainTex" },
            { Channel.Normal, "_Normal" },
            { Channel.Roughness, "_Roughness" },
            { Channel.Metallic, "_Metallic" },
            { Channel.Height, "_Displacement" }
        };
        private static Shader surfaceShader = Shader.Find ("SurfaceEdit/Advanced/TesselationDisplacementShader");
        private static Shader textureShader = Shader.Find ("SurfaceEdit/Advanced/TesselationDisplacementTextureShader");

        public float DisplacementIntensity
        {
            get => displacementIntensity;
            set => SetPropertyUndoRedo (v => displacementIntensity = v, () => displacementIntensity, value, true, v => Mathf.Clamp01 (v));
        }
        private float displacementIntensity = .1f;

        public float TesselationMultiplier
        {
            get => tesselationMultiplier;
            set => SetPropertyUndoRedo (v => tesselationMultiplier = v, () => tesselationMultiplier, value, true, v => Mathf.Clamp (v, 1, 64));
        }
        private float tesselationMultiplier = 5;

        public bool InvertNormal
        {
            get => invertNormal;
            set => SetProperty (ref invertNormal, value, true);
        }
        private bool invertNormal = false;

        public SurfaceRenderMode RenderMode
        {
            get => renderMode;
            set => SetProperty (ref renderMode, value, true);
        }
        private SurfaceRenderMode renderMode;

        public Channel RenderedChannel
        {
            get => renderedChannel;
            set => SetProperty (ref renderedChannel, value, renderMode == SurfaceRenderMode.Channel);
        }
        private Channel renderedChannel = Channel.Albedo;

        private GameObject go;
        private Renderer renderer;

        private Surface surface;

        public SurfaceVisualizer (UndoRedoRegister undoRedoRegister, Surface surface, SurfaceRenderMode renderMode = SurfaceRenderMode.Surface) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            this.renderMode = renderMode;
            this.surface = surface;

            go = new GameObject ("Surface Visualzation Plane");

            go.transform.Rotate (90, 0, 0);

            go.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildPlane (Vector2.one, new Vector2Int (128, 128)).ConvertToMesh ();
            var collider = go.AddComponent<BoxCollider> ();
            collider.size = new Vector3 (1, 1, .01f);
            collider.center = new Vector3 (.5f, .5f, 0);

            renderer = go.AddComponent<MeshRenderer> ();

            var go2 = new GameObject ("Base Level Plane");
            go2.transform.Translate (0, -0.01f, 0);
            go2.transform.Rotate (90, 0, 0);
            go2.AddComponent<MeshFilter> ().mesh = go.GetComponent<MeshFilter>().mesh;
            var renderer2 = go2.AddComponent<MeshRenderer> ();
            renderer2.material.shader = Shader.Find ("SurfaceEdit/Unlit/Transparent");
            renderer2.material.mainTexture = new SolidColorTextureProvider (new TextureResolution (TextureResolutionEnum.x32), new Color (.5f, .5f, .5f, .5f), false).Provide ();

            Update ();
            Changed += Update;
            surface.Context.Changed += (s, e) => UnityUpdateRegistrator.Instance.RegisterOneTimeActionOnUpdate (() => Update ());
        }

        public void Update () => Update (null, null);

        private void Update (object sender = null, EventArgs eventArgs = null)
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

                surface.Textures.TryGetValue(renderedChannel, out ProviderTexture providerTexture);
                var texture = providerTexture?.RenderTexture;
                if ( texture == null )
                    texture = new BlankChannelTextureProvider (new TextureResolution(TextureResolutionEnum.x32), renderedChannel, false).Provide();

                surface.Textures.TryGetValue (Channel.Height, out ProviderTexture providerHeight);
                var height = providerHeight?.RenderTexture;
                if ( height == null )
                    height = new BlankChannelTextureProvider (new TextureResolution (TextureResolutionEnum.x32), Channel.Height, false).Provide ();

                renderer.material.SetTexture ("_MainTex", texture);
                renderer.material.SetTexture ("_Displacement", height);
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