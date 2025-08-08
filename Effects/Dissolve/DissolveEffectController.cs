using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MantenseiLib;

namespace GameJam_HIKU.FX
{
    /// <summary>オブジェクト霧散エフェクト制御（Sprite & UI両対応）</summary>
    public class DissolveEffectController : MonoBehaviour
    {
        [field: SerializeField] public Material DissolveMaterial { get; private set; }
        [field: SerializeField] public float DissolveDuration { get; private set; } = 2f;
        [field: SerializeField] public AnimationCurve DissolveCurve { get; private set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [field: SerializeField] public bool DestroyOnComplete { get; private set; } = true;

        [GetComponent] public SpriteRenderer SpriteRenderer { get; private set; }
        [GetComponent] public Image UIImage { get; private set; }

        private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
        private Material runtimeMaterial;
        private Coroutine dissolveCoroutine;
        private bool isDissolving;

        public void Awake() 
        { 
            SetupRuntimeMaterial();
        }

        private void SetupRuntimeMaterial()
        {
            if (DissolveMaterial == null)
            {
                Debug.LogWarning($"DissolveMaterial is not assigned on {gameObject.name}");
                return;
            }

            // ランタイム用マテリアルを作成
            runtimeMaterial = new Material(DissolveMaterial);
            runtimeMaterial.SetFloat(DissolveAmountID, 0f);

            // SpriteRenderer または UI Image にマテリアルを適用
            if (SpriteRenderer != null)
            {
                SpriteRenderer.material = runtimeMaterial;
                Debug.Log($"Dissolve material applied to SpriteRenderer: {gameObject.name}");
            }
            else if (UIImage != null)
            {
                UIImage.material = runtimeMaterial;
                Debug.Log($"Dissolve material applied to UI Image: {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"No SpriteRenderer or Image component found on {gameObject.name}");
            }
        }

        /// <summary>霧散エフェクトを開始</summary>
        public void StartDissolve()
        {
            if (isDissolving || runtimeMaterial == null) return;

            if (dissolveCoroutine != null)
            {
                StopCoroutine(dissolveCoroutine);
            }
            dissolveCoroutine = StartCoroutine(DissolveCoroutine());
        }

        /// <summary>霧散エフェクトを即座に完了</summary>
        public void CompleteDissolve()
        {
            if (runtimeMaterial != null)
            {
                runtimeMaterial.SetFloat(DissolveAmountID, 1f);
            }

            if (DestroyOnComplete)
            {
                DestroyObject();
            }
        }

        private IEnumerator DissolveCoroutine()
        {
            isDissolving = true;

            float elapsed = 0f;

            while (elapsed < DissolveDuration)
            {
                float progress = elapsed / DissolveDuration;
                float dissolveAmount = DissolveCurve.Evaluate(progress);

                runtimeMaterial.SetFloat(DissolveAmountID, dissolveAmount);

                elapsed += Time.unscaledDeltaTime; // TimeScale = 0 対応
                yield return null;
            }

            runtimeMaterial.SetFloat(DissolveAmountID, 1f);

            if (DestroyOnComplete)
            {
                DestroyObject();
            }

            isDissolving = false;
            dissolveCoroutine = null;
        }

        private void DestroyObject()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            // ランタイムマテリアルをクリーンアップ
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
        }

        private void Update()
        {
            // テスト用：Dキーで霧散開始
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartDissolve();
            }
        }

        /// <summary>霧散中かどうか</summary>
        public bool IsDissolving => isDissolving;
    }
}