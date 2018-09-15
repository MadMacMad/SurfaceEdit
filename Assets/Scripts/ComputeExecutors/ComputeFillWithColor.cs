using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class ComputeFillWithColor : ComputeExecutor<UnityEngine.RenderTexture>
    {
        private UnityEngine.RenderTexture texture;
        private Color color;

        public ComputeFillWithColor (UnityEngine.RenderTexture texture, Color color) : base("Shaders/Compute/FillWithColor")
        {
            this.texture = texture;
            this.color = color;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", texture);
            shader.SetFloats ("TextureSize", texture.width, texture.height);
            shader.SetFloats ("Color", color.r, color.g, color.b, color.a);

            AutoDispatchDefaultShaderFunction (texture.width, texture.height, 8, 8);

            return texture;
        }
    }
}
