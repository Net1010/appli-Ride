using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeature_SettingsTransition : ScriptableRendererFeature
{
    [System.Serializable]
    public struct STransitionSettings
    {
        public RenderPassEvent Event;
        public Material mat;
    }
    public STransitionSettings settings = new STransitionSettings();
    SettingsTransitionPass settingsTransitionPass;

    // instantiates SettingsTransition pass
    public override void Create()
    {
        settingsTransitionPass = new SettingsTransitionPass(settings.Event, settings.mat);
    }

    // injects renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        settingsTransitionPass.Setup(renderingData.cameraData.renderer.cameraColorTarget);
        renderer.EnqueuePass(settingsTransitionPass);
    }
}
