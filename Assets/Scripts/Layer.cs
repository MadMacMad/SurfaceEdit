using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilify.TextureAffectors;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Layer : ObjectChangedRegistrator
    {
        public IReadOnlyList<ISurfaceAffector> SurfaceAffectors => surfaceAffectors.AsReadOnly();
        private List<ISurfaceAffector> surfaceAffectors;

        public Layer (UndoRedoRegister undoRedoRegister ) : base (undoRedoRegister)
        {

        }

        public void AddSurfaceAffector<T>(SurfaceAffector<T> surfaceAffector) where T : TextureAffector
        {
            if (!surfaceAffectors.Contains(surfaceAffector))
            {
                surfaceAffector.OnNeedUpdate += OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Add (surfaceAffector);
            }
            NotifyNeedUpdate ();
        }
        public void RemoveSurfaceAffector<T> (SurfaceAffector<T> surfaceAffector) where T : TextureAffector
        {
            if ( surfaceAffectors.Contains (surfaceAffector) )
            {
                surfaceAffector.OnNeedUpdate -= OnSurfaceAffectorNeedUpdate;
                surfaceAffectors.Remove (surfaceAffector as ISurfaceAffector);
            }
            NotifyNeedUpdate ();
        }

        private void OnSurfaceAffectorNeedUpdate(object sender)
            => NotifyNeedUpdate ();
    }
}
