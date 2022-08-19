namespace IL3DN
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Helping class to associate a name with a color
    /// </summary>
    [System.Serializable]
    public class ColorProperty
    {
        public Color color;
        public string name;

        public ColorProperty(Color color, string name)
        {
            this.color = color;
            this.name = name;
        }
    }

    /// <summary>
    /// Helping class to associate multiple colors to the same material
    /// </summary>
    [System.Serializable]
    public class MaterialColors
    {
        public List<ColorProperty> colors;

        public MaterialColors(Material material)
        {
            colors = new List<ColorProperty>();
#if UNITY_EDITOR
            Shader shader = material.shader;
            int nrOfProperties = UnityEditor.ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < nrOfProperties; i++)
            {
                if (UnityEditor.ShaderUtil.GetPropertyType(shader, i) == UnityEditor.ShaderUtil.ShaderPropertyType.Color)
                {
                    string propertyName = UnityEditor.ShaderUtil.GetPropertyName(shader, i);
                    Color color = material.GetColor(propertyName);
                    Debug.Log(propertyName + " " + color);
                    colors.Add(new ColorProperty(color, propertyName));
                }
            }
#endif
        }
    }

    /// <summary>
    /// Helping class to display multiple colors for a material
    /// </summary>
    [System.Serializable]
    public class MultipleColorProperties
    {
        public Material meterial;
        public List<MaterialColors> properties;
        public int selectedProperty;

        public MultipleColorProperties(Material material)
        {
            this.meterial = material;
            properties = new List<MaterialColors>();
            properties.Add(new MaterialColors(material));
        }
    }

    /// <summary>
    /// Displays a list of materials with multiple colors for customization 
    /// </summary>
    [RequireComponent(typeof(IL3DN_ColorController))]
    public class IL3DN_ColorManagerEffects : MonoBehaviour
    {
        public List<MultipleColorProperties> materials = new List<MultipleColorProperties>();

        public void Refresh()
        {
            for (int i = 0; i < materials.Count; i++)
            {
                for (int k = 0; k < materials[i].properties[materials[i].selectedProperty].colors.Count; k++)
                {
                    Color materialColor = materials[i].properties[materials[i].selectedProperty].colors[k].color;
                    string propertyName = materials[i].properties[materials[i].selectedProperty].colors[k].name;

                    materials[i].meterial.SetColor(propertyName, materialColor);
                }
            }
        }

        public void SetMaterialColors(int slot)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i].properties.Count > slot - 1)
                {
                    materials[i].selectedProperty = slot - 1;
                }
            }
            Refresh();
        }
    }
}