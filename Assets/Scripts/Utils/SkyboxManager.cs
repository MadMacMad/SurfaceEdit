using UnityEngine;

namespace SurfaceEdit
{
    public class SkyboxManager : Singleton<SkyboxManager>
    {
        public GameObject light;

        private Material skyboxMaterial;

        private SkyboxManager()
        {
            skyboxMaterial = new Material (Shader.Find ("SkyboxPlus/Cubemap"));
            RenderSettings.skybox = skyboxMaterial;
        }

        public void RotateSkyBoxIncremental (float rotation, bool rotateLight = true)
            => RotateSkybox (skyboxMaterial.GetVector("_Euler").y + rotation, rotateLight);

        public void RotateSkybox(float rotation, bool rotateLight = true)
        {
            var rotationVector = new Vector3 (0, rotation, 0);
            var quaterion = Quaternion.Euler (rotationVector.x, rotationVector.y, rotationVector.z);
            var matrix = Matrix4x4.TRS (Vector3.zero, quaterion, Vector3.one);
            skyboxMaterial.SetVector ("_Euler", rotationVector);
            skyboxMaterial.SetVector ("_Rotation1", matrix.GetRow (0));
            skyboxMaterial.SetVector ("_Rotation2", matrix.GetRow (1));
            skyboxMaterial.SetVector ("_Rotation3", matrix.GetRow (2));

            if ( light != null )
                light.transform.rotation = quaterion;
        }

        public void SetSkyboxCubeMap (Cubemap cubemap)
        {
            skyboxMaterial.mainTexture = cubemap;
            RenderSettings.customReflection = cubemap;
        }
        
        public void SetSkyboxTintColor(Color color)
            => skyboxMaterial.SetColor ("_Tint", color);
        
        public void SetSkyboxBlurAmount(float amount)
        {
            amount = Mathf.Clamp (amount, 0, 10);

            if (amount == 0)
                skyboxMaterial.SetFloat ("_Lod", 0);
            else
            {
                skyboxMaterial.SetFloat ("_Lod", 1);
                skyboxMaterial.SetFloat ("_LodLevel", amount);
            }
        }
        public void SetSkyboxExposure(float amount)
        {
            amount = Mathf.Clamp (amount, 0, 8);

            skyboxMaterial.SetFloat ("_Exposure", amount);
        }
        public void SetSkyboxSaturation(float amount)
        {
            amount = Mathf.Clamp (amount, 0, 2);

            skyboxMaterial.SetFloat ("_Saturation", amount);
        }
    }
}
