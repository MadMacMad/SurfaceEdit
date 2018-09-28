using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public delegate void NeedRenderEventHandler (object sender, NeedRenderEventArgs eventArgs);

    public interface INotifyNeedRender
    {
        event NeedRenderEventHandler NeedRender;
    }

    public class NeedRenderEventArgs : EventArgs
    {
        public readonly RenderContext renderContext;

        public NeedRenderEventArgs (RenderContext renderContext)
        {
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));
            this.renderContext = renderContext;
        }
    }
    
}
