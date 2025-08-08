using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameJam_HIKU
{
    /// <summary>URP�p�t���b�V���G�t�F�N�g Renderer Feature</summary>
    public class FlashRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class FlashSettings
        {
            public Material flashMaterial;
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public FlashSettings settings = new FlashSettings();
        private FlashRenderPass flashPass;

        public override void Create()
        {
            flashPass = new FlashRenderPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.flashMaterial != null)
            {
                // cameraColorTarget�̑���ɁARenderPass�̒��œK�؂ȃ^�[�Q�b�g���擾
                renderer.EnqueuePass(flashPass);
            }
        }
    }
}