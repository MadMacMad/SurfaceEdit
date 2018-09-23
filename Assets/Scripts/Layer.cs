using System;
using System.Collections.Generic;
using SurfaceEdit.TextureAffectors;

namespace SurfaceEdit
{
    public class Layer : PropertyChangedRegistrator, IDisposable
    {
        public LayerBlendType BlendType
        {
            get => blendType;
            set => SetPropertyUndoRedo (v => blendType = v, () => blendType, value, true);
        }
        private LayerBlendType blendType = LayerBlendType.HeightBlend;

        public IReadOnlyCollection<TextureChannel> Channels => channels.AsReadOnly();
        private List<TextureChannel> channels = new List<TextureChannel>();

        public IReadOnlyCollection<SurfaceAffector> SurfaceAffectors => surfaceAffectors.AsReadOnly();
        private List<SurfaceAffector> surfaceAffectors = new List<SurfaceAffector> ();
        
        public Layer (UndoRedoRegister undoRedoRegister) : base (undoRedoRegister) { }

        public void Process(Surface surface)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            foreach ( var affector in surfaceAffectors )
                affector.Affect (surface);
        }

        public void AddSurfaceAffector (TextureAffector textureAffector, TextureChannel channel)
        => AddSurfaceAffector (textureAffector.ToSurfaceAffector (channel));

        public void AddSurfaceAffector(SurfaceAffector surfaceAffector)
        {
            if (!surfaceAffectors.Contains(surfaceAffector))
            {
                surfaceAffector.NeedUpdate += OnChildNeedUpdate;
                surfaceAffectors.Add (surfaceAffector);
                NotifyNeedUpdate ();
            }
        }
        public void RemoveSurfaceAffector (SurfaceAffector surfaceAffector) 
        {
            if ( surfaceAffectors.Contains (surfaceAffector) )
            {
                surfaceAffector.NeedUpdate -= OnChildNeedUpdate;
                surfaceAffectors.Remove(surfaceAffector);
                NotifyNeedUpdate ();
            }
        }

        private void OnChildNeedUpdate(object sender, EventArgs eventArgs)
            => NotifyNeedUpdate ();

        public void Dispose()
        {

        }
    }
    public enum LayerBlendType
    {
        AlphaBlend,
        HeightBlend
    }
}
