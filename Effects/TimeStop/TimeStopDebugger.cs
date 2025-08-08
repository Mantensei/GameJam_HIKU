using UnityEngine;

namespace GameJam_HIKU.FX
{
    /// <summary>���Ԓ�~�G�t�F�N�g�̃f�o�b�O�p</summary>
    [RequireComponent(typeof(TimeStopEffectController))]
    public class TimeStopDebugger : MonoBehaviour
    {
        private TimeStopEffectController controller;

        [Header("�e�X�g�p�L�[")]
        [SerializeField] private KeyCode startKey = KeyCode.T;
        [SerializeField] private KeyCode endKey = KeyCode.Y;
        [SerializeField] private KeyCode toggleKey = KeyCode.Space;

        [Header("�}�e���A���m�F")]
        [SerializeField] private bool showMaterialInfo = true;

        private void Start()
        {
            controller = GetComponent<TimeStopEffectController>();

            // �}�e���A���̏�Ԃ��m�F
            if (controller.TimeStopMaterial != null && showMaterialInfo)
            {
                var mat = controller.TimeStopMaterial;
                Debug.Log("=== TimeStop Material Initial State ===");
                Debug.Log($"Shader: {mat.shader.name}");
                Debug.Log($"_Intensity: {mat.GetFloat("_Intensity")}");
                Debug.Log($"_TintColor: {mat.GetColor("_TintColor")}");
                Debug.Log($"_IdleNoiseStrength: {mat.GetFloat("_IdleNoiseStrength")}");
                Debug.Log($"_StartGlitchStrength: {mat.GetFloat("_StartGlitchStrength")}");

                // �K�v�ɉ����ċ����I�ɐF��ݒ�
                if (mat.GetColor("_TintColor") == Color.red ||
                    mat.GetColor("_TintColor").r > 0.9f)
                {
                    Debug.LogWarning("TintColor was red! Resetting to blue tint.");
                    mat.SetColor("_TintColor", new Color(0.8f, 0.9f, 1.0f, 1.0f));
                }
            }
        }

        private void Update()
        {
            // �e�X�g�p�̃L�[����
            if (Input.GetKeyDown(startKey))
            {
                Debug.Log("[TimeStop] Starting effect...");
                controller.StartTimeStop();
            }

            if (Input.GetKeyDown(endKey))
            {
                Debug.Log("[TimeStop] Ending effect...");
                controller.EndTimeStop();
            }

            if (Input.GetKeyDown(toggleKey))
            {
                Debug.Log("[TimeStop] Toggling effect...");
                controller.ToggleTimeStop();
            }

            // ���A���^�C���Œl���Ď��iF�L�[�j
            if (Input.GetKeyDown(KeyCode.F) && controller.TimeStopMaterial != null)
            {
                var mat = controller.TimeStopMaterial;
                Debug.Log("=== Current Material State ===");
                Debug.Log($"Active: {controller.IsTimeStopActive}");
                Debug.Log($"_Intensity: {mat.GetFloat("_Intensity")}");
                Debug.Log($"_AnimationTime: {mat.GetFloat("_AnimationTime")}");
                Debug.Log($"_TintColor: {mat.GetColor("_TintColor")}");
                Debug.Log($"Time.timeScale: {Time.timeScale}");
            }
        }
    }
}