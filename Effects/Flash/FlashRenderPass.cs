using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameJam_HIKU
{
    /// <summary>実際の描画処理を行うRenderPass</summary>
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

            // フラッシュ強度をチェック
            float flashIntensity = settings.flashMaterial.GetFloat(FlashIntensityID);
            if (flashIntensity <= 0f) return;

            CommandBuffer cmd = CommandBufferPool.Get(RenderTag);

            // 現在のカメラターゲットを取得
            RTHandle cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // 一時テクスチャを取得
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempTextureHandle, desc, name: "_TempFlashTexture");

            // フラッシュエフェクトを適用
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