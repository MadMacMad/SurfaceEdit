using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public abstract class ComputeExecutor<T>
    {
        protected ComputeShader shader;

        protected int DefaultFunctionID { get; private set; }

        private Dictionary<string, int> functionIDs = new Dictionary<string, int>();

        public ComputeExecutor (string shaderResourcesPath, string functionName) : this(shaderResourcesPath, new string[] { functionName }) { }
        public ComputeExecutor (string shaderResourcesPath) : this(shaderResourcesPath, new string[] { "CSMain" }) { }
        public ComputeExecutor (string shaderResourcesPath, string[] functionNames)
        {
            Assert.ArgumentNotNull (functionNames, nameof(functionNames));

            shader = (ComputeShader)Resources.Load (shaderResourcesPath);

            for (int i = 0; i < functionNames.Length; i++ )
                functionIDs[functionNames[i]] = shader.FindKernel (functionNames[i]);

            DefaultFunctionID = functionIDs["CSMain"];
        }

        protected int GetFunctionIDByName(string functionName)
        {
            return functionIDs[functionName];
        }
        protected void DispatchShaderFunction(string functionName, int threadGroupsX, int threadGroupsY, int threadGroupsZ = 1)
        {
            var functionID = GetFunctionIDByName (functionName);
            shader.Dispatch (functionID, threadGroupsX, threadGroupsY, threadGroupsZ);
        }
        protected void DispatchDefaultShaderFunction (int threadGroupsX, int threadGroupsY, int threadGroupsZ = 1)
        {
            var functionID = DefaultFunctionID;
            shader.Dispatch (functionID, threadGroupsX, threadGroupsY, threadGroupsZ);
        }
        protected void AutoDispatchDefaultShaderFunction (int textureWidth, int textureHeight, int threadsX, int threadsY)
        {
            AutoDispatchShaderFunction ("CSMain", textureWidth, textureHeight, threadsX, threadsY);
        }

        protected void AutoDispatchShaderFunction(string functionName, int textureWidth, int textureHeight, int threadsX, int threadsY)
        {
            var functionID = GetFunctionIDByName (functionName);

            var threadGroupsX = textureWidth / threadsX;
            var threadGroupsY = textureHeight / threadsY;

            if ( threadGroupsX <= 0 )
                threadGroupsX = 1;
            if ( threadGroupsY <= 0 )
                threadGroupsY = 1;

            shader.Dispatch (functionID, threadGroupsX, threadGroupsY, 1);
        }

        public abstract T Execute ();
    }
}
