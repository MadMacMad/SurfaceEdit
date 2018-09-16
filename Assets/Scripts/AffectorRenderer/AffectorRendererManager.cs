using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify.AffectorRenderer
{
    public class AffectorRendererManager : Singleton<AffectorRendererManager>
    {
        public static readonly int renderIgnoreLayerID;

        static AffectorRendererManager()
        {
            renderIgnoreLayerID = LayerMask.NameToLayer ("RenderIgnore");
        }

        public AffectorRendererManager() { }
        
        public void Allocate()
        {

        }
        public void Release(AffectorRendererStation station)
        {

        }

        public void Reset()
        {

        }
    }
}
