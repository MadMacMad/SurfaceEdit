using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class PartTextureComputeExecutor : ComputeExecutor<RenderTexture>
    {
        public Vector2Int Origin
        {
            get => origin;
            set
            {
                origin = value.ClampNew (Vector2Int.zero, new Vector2Int(int.MaxValue, int.MaxValue));
                shader.SetInts ("RenderOrigin", origin.x, origin.y);
            }
        }
        private Vector2Int origin;
        public Vector2Int Size
        {
            get => size;
            set
            {
                size = value.ClampNew (Vector2Int.one, new Vector2Int (int.MaxValue, int.MaxValue));
                shader.SetInts ("RenderSize", size.x, size.y);
            }
        }
        private Vector2Int size;

        protected RenderTexture mainTexture;
        
        public PartTextureComputeExecutor (RenderTexture mainTexture, string shaderResourcesPath) : base (shaderResourcesPath)
        {
            Assert.ArgumentNotNull (mainTexture, nameof (mainTexture));
            this.mainTexture = mainTexture;

            origin = Vector2Int.zero;
            shader.SetInts ("RenderOrigin", origin.x, origin.y);

            size = mainTexture.GetVectorSize();
            shader.SetInts ("RenderSize", size.x, size.y);
        }

        public override RenderTexture Execute()
        {
            DispatchShader ();
            return mainTexture;
        }
        
        protected void DispatchShader ()
        {
            DispatchShader (Size.x, Size.y);
        }
    }
}
