using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MantenseiLib;

namespace GameJam_HIKU.FX
{
    /// <summary>�I�u�W�F�N�g���U�G�t�F�N�g����iSprite & UI���Ή��j</summary>
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

            // �����^�C���p�}�e���A�����쐬
            runtimeMaterial = new Material(DissolveMaterial);
            runtimeMaterial.SetFloat(DissolveAmountID, 0f);

            // SpriteRenderer �܂��� UI Image �Ƀ}�e���A����K�p
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

        /// <summary>���U�G�t�F�N�g���J�n</summary>
        public void StartDissolve()
        {
            if (isDissolving || runtimeMaterial == null) return;

            if (dissolveCoroutine != null)
            {
                StopCoroutine(dissolveCoroutine);
            }
            dissolveCoroutine = StartCoroutine(DissolveCoroutine());
        }

        /// <summary>���U�G�t�F�N�g�𑦍��Ɋ���</summary>
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

                elapsed += Time.unscaledDeltaTime; // TimeScale = 0 �Ή�
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
            // �����^�C���}�e���A�����N���[���A�b�v
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
        }

        private void Update()
        {
            // �e�X�g�p�FD�L�[�Ŗ��U�J�n
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartDissolve();
            }
        }

        /// <summary>���U�����ǂ���</summary>
        public bool IsDissolving => isDissolving;
    }
}