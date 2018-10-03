using System;
using UnityEngine;

namespace SurfaceEdit
{
    public class SkyboxManager : Singleton<SkyboxManager>
    {
        public event Action OnRotate;

        public float Rotation => skyboxMaterial.GetVector ("_Euler").y;
        public float Blurriness => skyboxMaterial.GetFloat ("_LodLevel") / 10f;

        public GameObject light;

        private Material skyboxMaterial;

        private SkyboxManager()
        {
            skyboxMaterial = new Material (Shader.Find ("SkyboxPlus/Cubemap"));
            RenderSettings.skybox = skyboxMaterial;
            SetSkyboxBlurAmount (.2f);
        }

        public void RotateSkyBoxIncremental (float rotation, bool rotateLight = true)
            => RotateSkybox (skyboxMaterial.GetVector("_Euler").y + rotation, rotateLight);

        public void RotateSkybox(float rotation, bool rotateLight = true)
        {
            if ( rotation > 360 )
                rotation = rotation % 360;
            else if ( rotation < 0 )
                rotation = 360 - Mathf.Abs (rotation) % 360;

            var rotationVector = new Vector3 (0, rotation, 0);
            var quaterion = Quaternion.Euler (rotationVector.x, rotationVector.y, rotationVector.z);
            var matrix = Matrix4x4.TRS (Vector3.zero, quaterion, Vector3.one);
            skyboxMaterial.SetVector ("_Euler", rotationVector);
            skyboxMaterial.SetVector ("_Rotation1", matrix.GetRow (0));
            skyboxMaterial.SetVector ("_Rotation2", matrix.GetRow (1));
            skyboxMaterial.SetVector ("_Rotation3", matrix.GetRow (2));

            if ( light != null )
                light.transform.rotation = quaterion;

            OnRotate?.Invoke ();
        }

        public void SetSkyboxCubeMap (Cubemap cubemap)
        {
            skyboxMaterial.SetTexture("_Tex", cubemap);
            RenderSettings.customReflection = cubemap;
        }
        
        public void SetSkyboxTintColor(Color color)
            => skyboxMaterial.SetColor ("_Tint", color);
        
        public void SetSkyboxBlurAmount(float amount)
        {
            amount = Mathf.Lerp (0, 10, amount);
            skyboxMaterial.SetFloat ("_LodLevel", amount);
        }
        public void SetSkyboxExposure(float amount)
        {
            amount = Mathf.Lerp (0, 8, amount);

            skyboxMaterial.SetFloat ("_Exposure", amount);
        }
        public void SetSkyboxSaturation(float amount)
        {
            amount = Mathf.Clamp (amount, 0, 2);

            skyboxMaterial.SetFloat ("_Saturation", amount);
        }
    }
}
