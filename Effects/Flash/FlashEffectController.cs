using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameJam_HIKU
{
    /// <summary>URP用フラッシュエフェクト制御</summary>
    public class FlashEffectController : MonoBehaviour
    {
        [field: SerializeField] public Material FlashMaterial { get; private set; }
        [field: SerializeField] public float FlashDuration { get; private set; } = 0.1f;
        [field: SerializeField] public AnimationCurve FlashCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        private static readonly int FlashIntensityID = Shader.PropertyToID("_FlashIntensity");
        private Coroutine flashCoroutine;

        private void Awake()
        {
            if (FlashMaterial != null)
            {
                FlashMaterial.SetFloat(FlashIntensityID, 0f);
            }
        }

        /// <summary>フラッシュエフェクトを発動</summary>
        public void TriggerFlash()
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {            
            float elapsed = 0f;

            while (elapsed < FlashDuration)
            {
                float progress = elapsed / FlashDuration;
                float intensity = FlashCurve.Evaluate(progress);

                FlashMaterial.SetFloat(FlashIntensityID, intensity);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            FlashMaterial.SetFloat(FlashIntensityID, 0f);
            flashCoroutine = null;
        }
    }
}
