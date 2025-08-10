using MantenseiLib.Audio;
using System.Collections;
using UnityEngine;

namespace GameJam_HIKU.FX
{
    /// <summary>���Ԓ�~�G�t�F�N�g����</summary>
    public class TimeStopEffectController : MonoBehaviour
    {
        [field: SerializeField] public Material TimeStopMaterial { get; private set; }
        [field: SerializeField] public float FadeInDuration { get; private set; } = 0.5f;
        [field: SerializeField] public float FadeOutDuration { get; private set; } = 0.3f;
        [field: SerializeField] public AnimationCurve FadeCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private static readonly int TimeStopIntensityID = Shader.PropertyToID("_Intensity");
        private static readonly int AnimationTimeID = Shader.PropertyToID("_AnimationTime");

        private Coroutine timeStopCoroutine;
        private bool isTimeStopActive;
        private float totalTime = 0f;

        private void Awake()
        {
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
                TimeStopMaterial.SetFloat(AnimationTimeID, 0f);
            }
        }

        private void Update()
        {
            // ��Ɏ��Ԃ��X�V�iTimeScale�֌W�Ȃ������j
            totalTime += Time.unscaledDeltaTime;

            if (TimeStopMaterial != null)
            {
                // �V�F�[�_�[�Ɍ��݂̎��Ԃ𑗂�i����Ńm�C�Y�������j
                TimeStopMaterial.SetFloat(AnimationTimeID, totalTime);
            }
        }

        private void OnDisable()
        {
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
            }
        }

        private void OnDestroy()
        {
            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0f);
            }
        }

        /// <summary>���Ԓ�~�G�t�F�N�g���J�n</summary>
        public void StartTimeStop()
        {
            GetComponent<SEPlayer>()?.ForcePlaySE();

            if (timeStopCoroutine != null)
            {
                StopCoroutine(timeStopCoroutine);
            }
            timeStopCoroutine = StartCoroutine(FadeInCoroutine());
        }

        /// <summary>���Ԓ�~�G�t�F�N�g���I��</summary>
        public void EndTimeStop()
        {
            if (timeStopCoroutine != null)
            {
                StopCoroutine(timeStopCoroutine);
            }
            timeStopCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        /// <summary>���Ԓ�~�̏�Ԃ��g�O��</summary>
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
            isTimeStopActive = true;
            float elapsed = 0f;

            while (elapsed < FadeInDuration)
            {
                float progress = elapsed / FadeInDuration;
                float intensity = FadeCurve.Evaluate(progress) * 0.2f; // 0.2�܂�

                if (TimeStopMaterial != null)
                {
                    TimeStopMaterial.SetFloat(TimeStopIntensityID, intensity);
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (TimeStopMaterial != null)
            {
                TimeStopMaterial.SetFloat(TimeStopIntensityID, 0.2f);
            }

            timeStopCoroutine = null;
        }

        private IEnumerator FadeOutCoroutine()
        {
            isTimeStopActive = false;
            float elapsed = 0f;
            float startIntensity = TimeStopMaterial != null ? TimeStopMaterial.GetFloat(TimeStopIntensityID) : 0.2f;

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

            timeStopCoroutine = null;
        }

        /// <summary>���Ԓ�~���L�����ǂ���</summary>
        public bool IsTimeStopActive => isTimeStopActive;
    }
}