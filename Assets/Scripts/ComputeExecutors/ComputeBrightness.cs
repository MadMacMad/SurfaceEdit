﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class ComputeBrighntess : ComputeExecutor<RenderTexture>
    {
        private RenderTexture texture;
        private float brightnessFactor;

        public ComputeBrighntess (RenderTexture texture, float brightnessFactor) : base("Shaders/Compute/Brightness")
        {
            this.texture = texture;
            this.brightnessFactor = brightnessFactor;
        }

        public override RenderTexture Execute ()
        {
            shader.SetTexture (DefaultFunctionID, "Result", texture);
            shader.SetFloats ("TextureSize", texture.width, texture.height);
            shader.SetFloat ("Brightness", brightnessFactor);

            AutoDispatchDefaultShaderFunction (texture.width, texture.height, 8, 8);

            return texture;
        }
    }
}
