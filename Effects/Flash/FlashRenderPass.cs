using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameJam_HIKU
{
    /// <summary>���ۂ̕`�揈�����s��RenderPass</summary>
    public class FlashRenderPass : ScriptableRenderPass
    {
        private static readonly string RenderTag = "FlashEffect";
        private static readonly int FlashIntensityID = Shader.PropertyToID("_Intensity");

        private FlashRendererFeature.FlashSettings settings;
        private RTHandle tempTextureHandle;

        public FlashRenderPass(FlashRendererFeature.FlashSettings settings)
        {
            this.settings = settings;
            renderPassEvent = settings.renderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.flashMaterial == null) return;

            // �t���b�V�����x���`�F�b�N
            float flashIntensity = settings.flashMaterial.GetFloat(FlashIntensityID);
            if (flashIntensity <= 0f) return;

            CommandBuffer cmd = CommandBufferPool.Get(RenderTag);

            // ���݂̃J�����^�[�Q�b�g���擾
            RTHandle cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // �ꎞ�e�N�X�`�����擾
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempTextureHandle, desc, name: "_TempFlashTexture");

            // �t���b�V���G�t�F�N�g��K�p
            cmd.Blit(cameraColorTarget, tempTextureHandle, settings.flashMaterial);
            cmd.Blit(tempTextureHandle, cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnFinishCameraStackRendering(CommandBuffer cmd)
        {
            tempTextureHandle?.Release();
        }

        public void Dispose()
        {
            tempTextureHandle?.Release();
        }
    }
}