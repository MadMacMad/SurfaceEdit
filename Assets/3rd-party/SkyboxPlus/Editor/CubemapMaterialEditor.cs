using UnityEngine;
using UnityEditor;

namespace SkyboxPlus
{
    public class CubemapMaterialEditor : ShaderGUI
    {
        MaterialProperty cubemap;
        MaterialProperty tint;
        MaterialProperty euler;
        MaterialProperty exposure;
        MaterialProperty saturation;
        MaterialProperty lod;
        MaterialProperty lodLevel;

        static GUIContent textCubemap = new GUIContent("Cubemap");

        bool initial = true;

        void FindProperties(MaterialProperty[] props)
        {
            cubemap = FindProperty("_Tex", props);
            tint = FindProperty("_Tint", props);
            euler = FindProperty("_Euler", props);
            exposure = FindProperty("_Exposure", props);
            saturation = FindProperty("_Saturation", props);
            lod = FindProperty("_Lod", props);
            lodLevel = FindProperty("_LodLevel", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);
            if (ShaderPropertiesGUI(materialEditor) || initial )
                foreach (Material m in materialEditor.targets)
                    SetMatrix(m);
            initial = false;
        }

        bool ShaderPropertiesGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            materialEditor.TexturePropertySingleLine(textCubemap, cubemap, tint);
            Vector3Property(materialEditor, euler, "Rotation");
            materialEditor.ShaderProperty(exposure, "Exposure");
            materialEditor.ShaderProperty(saturation, "Saturation");

            materialEditor.ShaderProperty(lod, "Specify MIP Level");
            if ( lod.hasMixedValue || lod.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(lodLevel, "Level");
                EditorGUI.indentLevel--;
            }

            return EditorGUI.EndChangeCheck();
        }

        static void SetMatrix(Material material)
        {
            var r = material.GetVector("_Euler");
            var q = Quaternion.Euler(r.x, r.y, r.z);
            var m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            material.SetVector("_Rotation1", m.GetRow(0));
            material.SetVector("_Rotation2", m.GetRow(1));
            material.SetVector("_Rotation3", m.GetRow(2));
        }

        void Vector3Property(MaterialEditor materialEditor, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var newValue = EditorGUILayout.Vector3Field(label, prop.vectorValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck()) prop.vectorValue = newValue;
        }
    }
}
