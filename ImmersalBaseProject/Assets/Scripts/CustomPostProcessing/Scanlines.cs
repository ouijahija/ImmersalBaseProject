using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScanlinesRenderer), PostProcessEvent.AfterStack, "Custom/Scanlines")]
public sealed class Scanlines : PostProcessEffectSettings
{
    public IntParameter width = new IntParameter { value = 2 };
    public IntParameter referenceScreenHeight = new IntParameter { value = 1920 };

    //[Range(0f, 1f), Tooltip("FMIN")]
    //public FloatParameter fmin = new FloatParameter { value = 0.5f };

    public ColorParameter color = new ColorParameter { value = new Color(0, 0, 0, 0.5f) };
}

public sealed class ScanlinesRenderer : PostProcessEffectRenderer<Scanlines>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Scanlines"));
        sheet.properties.SetInt("_Width", settings.width);
        sheet.properties.SetInt("_RefScreenHeight", settings.referenceScreenHeight);
        //sheet.properties.SetFloat("fmin", settings.fmin);
        sheet.properties.SetColor("_LineColor", settings.color);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}