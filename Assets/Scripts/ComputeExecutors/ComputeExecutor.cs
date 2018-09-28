using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class ComputeExecutor<T>
    {
        protected ComputeShader shader;

        protected int ShaderFunctionID { get; private set; }

        public ComputeExecutor (string shaderResourcesPath)
        {
            shader = (ComputeShader)Resources.Load (shaderResourcesPath);

            Assert.ArgumentTrue (shader != null, nameof (shaderResourcesPath) + " is incorrect. Unable to load shader");

            ShaderFunctionID = shader.FindKernel("CSMain");
        }
        
        protected void DispatchShader(int textureWidth, int textureHeight)
        {
            var threadGroupsX = textureWidth / 32;
            var threadGroupsY = textureHeight / 32;

            if ( threadGroupsX <= 0 )
                threadGroupsX = 1;
            if ( threadGroupsY <= 0 )
                threadGroupsY = 1;

            shader.Dispatch (ShaderFunctionID, threadGroupsX, threadGroupsY, 1);
        }

        public abstract T Execute ();
    }
}
