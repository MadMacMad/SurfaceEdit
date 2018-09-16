using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.TextureAffectors;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Layer : ObjectChangedRegistrator, IDisposable
    {
        public IReadOnlyList<ISurfaceAffector> SurfaceAffectors => surfaceAffectors.AsReadOnly();
        private List<ISurfaceAffector> surfaceAffectors = new List<ISurfaceAffector>();

        public bool IsUseMask { get => isUseMask; set => SetPropertyAndRegisterUndoRedo (v => isUseMask = v, () => isUseMask, value, true); }
        private bool isUseMask;

        private Mask mask;
        private List<TextureAffector> maskAffectors = new List<TextureAffector>();

        public Layer (UndoRedoRegister undoRedoRegister, Mask mask) : base (undoRedoRegister)
        {
            Assert.ArgumentNotNull (mask, nameof (mask));
            this.mask = mask;
        }

        public void Process(Surface surface)
        {
            Assert.ArgumentNotNull (surface, nameof (surface));

            foreach ( var affector in surfaceAffectors )
                affector.Affect (surface);
        }

        public void AddSurfaceAffector<T>(SurfaceAffector<T> surfaceAffector) where T : TextureAffector
        {
            if (!surfaceAffectors.Contains(surfaceAffector))
            {
                surfaceAffector.OnNeedUpdate += OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Add (surfaceAffector);
                NotifyNeedUpdate ();
            }
        }
        public void RemoveSurfaceAffector<T> (SurfaceAffector<T> surfaceAffector) where T : TextureAffector
        {
            if ( surfaceAffectors.Contains (surfaceAffector) )
            {
                surfaceAffector.OnNeedUpdate -= OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Remove (surfaceAffector as ISurfaceAffector);
                NotifyNeedUpdate ();
            }
        }

        public void AddMaskAffector(TextureAffector textureAffector)
        {

        }

        private void OnSurfaceAffectorNeedUpdate(object sender)
            => NotifyNeedUpdate ();

        public void Dispose()
        {

        }
    }
}
