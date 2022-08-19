// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(IL3DNFogPPPPSRenderer), PostProcessEvent.BeforeStack, "IL3DN/Fog", true)]
public sealed class IL3DN_Fog_PP : PostProcessEffectSettings
{
    [Range(0f, 100f), Tooltip("Density")]
    public FloatParameter _Density = new FloatParameter { value = 20f };
    [Tooltip("Near Color")]
    public ColorParameter _NearColor = new ColorParameter { value = new Color(1f, 0.203827f, 0f, 0f) };
    [Tooltip("Far Color")]
    public ColorParameter _FarColor = new ColorParameter { value = new Color(0f, 0.2739539f, 1f, 0f) };
    [Tooltip("Glow Color")]
    public ColorParameter _GlowColor = new ColorParameter { value = new Color(1f, 0.203827f, 0f, 0f) };
    [Tooltip("Exclude Skybox")]
    public BoolParameter _ExcludeSkybox = new BoolParameter { value = true };
}

public sealed class IL3DNFogPPPPSRenderer : PostProcessEffectRenderer<IL3DN_Fog_PP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("IL3DN/FogPP"));
        sheet.properties.SetFloat("_Density", settings._Density);
        sheet.properties.SetColor("_NearColor", settings._NearColor);
        sheet.properties.SetColor("_FarColor", settings._FarColor);
        sheet.properties.SetColor("_GlowColor", settings._GlowColor);

        if (settings._ExcludeSkybox == true)
        {
            Shader.EnableKeyword("_EXCLUDESKYBOX_ON");
        }
        else
        {
            Shader.DisableKeyword("_EXCLUDESKYBOX_ON");
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
#endif
