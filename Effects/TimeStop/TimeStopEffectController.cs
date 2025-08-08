using System.Collections;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>時間停止エフェクト制御</summary>
    public class TimeStopEffectController : MonoBehaviour
    {
        [field: SerializeField] public Material TimeStopMaterial { get; private set; }
        [field: SerializeField] public float FadeInDuration { get; private set; } = 0.5f;
        [field: SerializeField] public float FadeOutDuration { get; private set; } = 0.3f;
        [field: SerializeField] public AnimationCurve FadeCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private static readonly int TimeStopIntensityID = Shader.PropertyToID("_Intensity");
        private static readonly int WaveRadiusID = Shader.PropertyToID("_WaveRadius");
        private Coroutine timeStopCoroutine;
        private bool isTimeStopActive;

        private void Awake()
        {
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
                TimeStopMaterial.SetFloat(WaveRadiusID, 0f);
            }
        }

        private void OnDisable()
        {
            // プレイモード終了時にエフェクトをリセット
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
                TimeStopMaterial.SetFloat(WaveRadiusID, 0f);
            }
        }

        private void OnDestroy()
        {
            // オブジェクト削除時にもエフェクトをリセット
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
                TimeStopMaterial.SetFloat(WaveRadiusID, 0f);
            }
        }

        /// <summary>時間停止エフェクトを開始</summary>
        public void StartTimeStop()
        {
            if (timeStopCoroutine != null)
            {
                StopCoroutine(timeStopCoroutine);
            }
            timeStopCoroutine = StartCoroutine(FadeInCoroutine());
        }

        /// <summary>時間停止エフェクトを終了</summary>
        public void EndTimeStop()
        {
            if (timeStopCoroutine != null)
            {
                StopCoroutine(timeStopCoroutine);
            }
            timeStopCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        /// <summary>時間停止の状態をトグル</summary>
        public void ToggleTimeStop()
        {
            if (isTimeStopActive)
            {
                EndTimeStop();
            }
            else
            {
                StartTimeStop();
            }
        }

        private IEnumerator FadeInCoroutine()
        {
            Debug.Log("Time Stop started!");
            isTimeStopActive = true;

            float elapsed = 0f;

            while (elapsed < FadeInDuration)
            {
                float progress = elapsed / FadeInDuration;
                float intensity = FadeCurve.Evaluate(progress);

                if (TimeStopMaterial != null)
                {
                    TimeStopMaterial.SetFloat(TimeStopIntensityID, intensity);
                    // 波紋は最初は速く拡散、後半はゆっくり
                    float waveRadius = Mathf.Pow(intensity, 0.7f) * 1.4f;
                    TimeStopMaterial.SetFloat(WaveRadiusID, waveRadius);
                }

                elapsed += Time.unscaledDeltaTime; // TimeScale = 0 対応
                yield return null;
            }

            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 1f);
            }

            Debug.Log("Time Stop fade in complete");
            timeStopCoroutine = null;
        }

        private IEnumerator FadeOutCoroutine()
        {
            Debug.Log("Time Stop ending...");
            isTimeStopActive = false;

            float elapsed = 0f;
            float startIntensity = TimeStopMaterial != null ? TimeStopMaterial.GetFloat(TimeStopIntensityID) : 1f;

            while (elapsed < FadeOutDuration)
            {
                float progress = elapsed / FadeOutDuration;
                float intensity = Mathf.Lerp(startIntensity, 0f, FadeCurve.Evaluate(progress));

                if (TimeStopMaterial != null)
                {
                    TimeStopMaterial.SetFloat(TimeStopIntensityID, intensity);
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
            }

            Debug.Log("Time Stop ended");
            timeStopCoroutine = null;
        }

        private void Update()
        {
            // テスト用コード削除
        }

        /// <summary>時間停止が有効かどうか</summary>
        public bool IsTimeStopActive => isTimeStopActive;
    }
}