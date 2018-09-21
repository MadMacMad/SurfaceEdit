﻿using System;
using System.Collections.Generic;
using SurfaceEdit.TextureAffectors;

namespace SurfaceEdit
{
    public class Layer : PropertyChangedRegistrator, IDisposable
    {
        public IReadOnlyList<ISurfaceAffector> SurfaceAffectors => surfaceAffectors.AsReadOnly();
        private List<ISurfaceAffector> surfaceAffectors = new List<ISurfaceAffector>();

        public bool IsUseMask { get => isUseMask; set => SetPropertyUndoRedo (v => isUseMask = v, () => isUseMask, value, true); }
        private bool isUseMask;

        public Layer (UndoRedoRegister undoRedoRegister) : base (undoRedoRegister)
        {
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
                surfaceAffector.NeedUpdate += OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Add (surfaceAffector);
                NotifyNeedUpdate ();
            }
        }
        public void RemoveSurfaceAffector<T> (SurfaceAffector<T> surfaceAffector) where T : TextureAffector
        {
            if ( surfaceAffectors.Contains (surfaceAffector) )
            {
                surfaceAffector.NeedUpdate -= OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Remove (surfaceAffector as ISurfaceAffector);
                NotifyNeedUpdate ();
            }
        }

        private void OnSurfaceAffectorNeedUpdate(object sender)
            => NotifyNeedUpdate ();

        public void Dispose()
        {

        }
    }
}
