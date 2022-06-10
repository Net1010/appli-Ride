using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsTransitionPass : ScriptableRenderPass
{
    public Material mat;
    const string profilertag = "SettingsTransitionPass";
    RenderTargetHandle temp1Handle;
    RenderTargetIdentifier colorBuffer, temporaryBuffer;
    int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

    // constructor
    public SettingsTransitionPass(RenderPassEvent renderPassEvent, Material mat)
    {
        this.renderPassEvent = renderPassEvent;
        this.mat = mat;
        temp1Handle.Init("Temp Handle 1");
    }
    public void Setup(RenderTargetIdentifier cameraTarget)
    {
    }

    // method is called by the renderer before executing the render pass
    // cameraTextureDescriptor - holds info the texture the camera is rendering to (width, height, depth, etc)
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //camSettings.enableRandomWrite = true;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // Grab the camera target descriptor. We will use this when creating a temporary render texture.
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        // Set the number of depth bits we need for our temporary render texture.
        descriptor.depthBufferBits = 0;

        // Enable these if your pass requires access to the CameraDepthTexture or the CameraNormalsTexture.
        // ConfigureInput(ScriptableRenderPassInput.Depth);
        // ConfigureInput(ScriptableRenderPassInput.Normal);

        // Grab the color buffer from the renderer camera color target.
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

        // Create a temporary render texture using the descriptor from above.
        cmd.GetTemporaryRT(temporaryBufferID, descriptor, FilterMode.Bilinear);
        temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);
    }


    // create command buffer then ship it off
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler(profilertag)))
        {
            // Blit from the color buffer to a temporary buffer and back. This is needed for a two-pass shader.
            Blit(cmd, colorBuffer, temporaryBuffer, mat);
            Blit(cmd, temporaryBuffer, colorBuffer, mat); 
        }

        // Execute the command buffer and release it.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
