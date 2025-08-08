

using UnityEngine;

/// <summary>OnRenderImageを実行する専用コンポーネント</summary>
namespace GameJam_HIKU
{
    public class FlashRenderer : MonoBehaviour
    {
        private Material flashMaterial;
        private bool flashEnabled;

        public void SetFlashMaterial(Material material)
        {
            flashMaterial = material;
            Debug.Log($"FlashRenderer material set: {material != null}");
        }

        public void SetFlashEnabled(bool enabled)
        {
            flashEnabled = enabled;
            Debug.Log($"Flash enabled: {enabled}");
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Debug.Log($"OnRenderImage called - Flash enabled: {flashEnabled}");

            if (flashEnabled && flashMaterial != null)
            {
                Graphics.Blit(src, dest, flashMaterial);
                Debug.Log("Flash blit executed");
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    } 
}