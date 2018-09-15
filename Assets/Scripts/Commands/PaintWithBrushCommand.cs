using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tilify.Commands
{
    public class PaintWithBrushCommand : ICommand
    {
        private UnityEngine.RenderTexture texture;
        private Brush brush;
        private Vector2Int brushOrigin;
        private Action<UnityEngine.RenderTexture> setTextureAction;
        public PaintWithBrushCommand (UnityEngine.RenderTexture texture, Brush brush, Vector2Int brushOrigin, Action<UnityEngine.RenderTexture> setTextureAction)
        {
            this.texture = texture.Copy();
            this.setTextureAction = setTextureAction;
            this.brush = brush;
            this.brushOrigin = brushOrigin;
        }

        private RenderTexture GetTexture()
        {
            if ( texture.IsCreated() )
                return texture;
            return texture;
        }

        public void Do ()
        {
            //m_SetTextureAction (new ComputePaintWithBrushWrapped (GetTexture(), m_Brush, m_BrushOrigin).Execute ());
        }

        public void Undo ()
        {
            setTextureAction (GetTexture ());
        }

        public void Dispose ()
        {
            brush.Dispose ();
        }
    }
}
