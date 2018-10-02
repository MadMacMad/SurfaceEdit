using System;
using System.Collections.Generic;
using System.Linq;
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

        private static RenderTexture blankHeightTexture =
            new BlankChannelTextureProvider (new TextureResolution (TextureResolutionEnum.x32),Channel.Height, false) .Provide ();

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

        public Channel ChannelToRender
        {
            get => channelToRender;
            set => SetPropertyValidate (ref channelToRender, value, renderMode == SurfaceRenderMode.Channel,
                                        c => new Tuple<bool, Channel> (channels.List.Contains(c), c));
        }
        private Channel channelToRender = Channel.Albedo;

        private GameObject surfacePlane;
        private GameObject baseLevelPlane;
        private Renderer surfacePlaneRenderer;

        private Surface surface;

        private Channels channels;

        public SurfaceVisualizer (UndoRedoRegister undoRedoRegister, Surface surface, SurfaceRenderMode renderMode = SurfaceRenderMode.Surface) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            this.renderMode = renderMode;
            this.surface = surface;

            channels = surface.Context.Channels;

            CreateSurfacePlane ();
            CreateBaseLevelPlane ();

            Update ();
            Changed += (s, e) => Update ();
            surface.Context.Changed += (s, e) => UnityUpdateRegistrator.Instance.RegisterOneTimeActionOnUpdate (Update);
        }

        private void CreateSurfacePlane ()
        {
            surfacePlane = new GameObject ("SurfacePlane");

            surfacePlane.transform.Rotate (90, 0, 0);

            surfacePlane.AddComponent<MeshFilter> ().mesh = MeshBuilder.BuildPlane (Vector2.one, new Vector2Int (128, 128)).ConvertToMesh ();

            surfacePlaneRenderer = surfacePlane.AddComponent<MeshRenderer> ();
        }
        private void CreateBaseLevelPlane ()
        {
            baseLevelPlane = new GameObject ("BaseLevelPlane");

            baseLevelPlane.transform.Rotate (90, 0, 0);

            baseLevelPlane.AddComponent<MeshFilter> ().mesh = surfacePlane.GetComponent<MeshFilter> ().mesh;
            var baseLevelRenderer = baseLevelPlane.AddComponent<MeshRenderer> ();
            baseLevelRenderer.material.shader = Shader.Find ("SurfaceEdit/Unlit/Transparent");

            var collider = baseLevelPlane.AddComponent<BoxCollider> ();
            collider.size = new Vector3 (1, 1, .01f);
            collider.center = new Vector3 (.5f, .5f, 0);

            var greyTexture = new SolidColorTextureProvider (new TextureResolution (TextureResolutionEnum.x32), new Color (.5f, .5f, .5f, .5f), false)
                .Provide ()
                .ConvertToTextureAndRelease ();

            baseLevelRenderer.material.mainTexture = greyTexture;
        }

        public void Update ()
        {
            if ( renderMode == SurfaceRenderMode.Surface )
            {
                surfacePlaneRenderer.material.shader = surfaceShader;
                foreach ( var pair in surface.Textures )
                    if ( channelPropertyPair.ContainsKey (pair.Key) )
                        surfacePlaneRenderer.material.SetTexture (channelPropertyPair[pair.Key], pair.Value);
            }
            else
            {
                surfacePlaneRenderer.material.shader = textureShader;

                surface.Textures.TryGetValue (channelToRender, out ProviderTexture providerTexture);
                var texture = providerTexture.RenderTexture;

                surface.Textures.TryGetValue (Channel.Height, out ProviderTexture providerHeight);
                var height = providerHeight?.RenderTexture ?? blankHeightTexture;

                surfacePlaneRenderer.material.SetTexture ("_MainTex", texture);
                surfacePlaneRenderer.material.SetTexture ("_Displacement", height);
            }

            surfacePlaneRenderer.material.SetFloat ("_TesselationMultiplier", tesselationMultiplier);
            surfacePlaneRenderer.material.SetFloat ("_DisplacementIntensity", displacementIntensity);
            surfacePlaneRenderer.material.SetFloat ("_InvertNormal", ( invertNormal ) ? 1 : 0);
        }

        public void CycleChannelToRenderNext ()
        {
            var newChannelID = (int)channelToRender + 1;
            Channel newChannel = default;
            var maxID = Utils.EnumCount<Channel> ();
            do
            {
                if ( newChannelID >= maxID )
                    newChannelID = 0;

                newChannel = (Channel)newChannelID;
                newChannelID += 1;
            }
            while ( !channels.List.Contains (newChannel) );

            ChannelToRender = newChannel;
        }
        public void CycleChannelToRenderPrevious ()
        {
            var newChannelID = (int)channelToRender - 1;
            Channel newChannel = default;
            var maxID = Utils.EnumCount<Channel> ();
            do
            {
                if ( newChannelID < 0 )
                    newChannelID = maxID;

                newChannel = (Channel)newChannelID;
                newChannelID -= 1;
            }
            while ( !channels.List.Contains (newChannel) );

            ChannelToRender = newChannel;
        }

        public void Dispose ()
        {
            GameObject.Destroy (surfacePlane);
            GameObject.Destroy (baseLevelPlane);
        }
    }

    public enum SurfaceRenderMode
    {
        Surface,
        Channel
    }
}