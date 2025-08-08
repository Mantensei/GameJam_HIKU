using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameJam_HIKU.FX
{
    public class TimeStopDebugger : MonoBehaviour
    {
        [SerializeField] private Material timeStopMaterial;

        void Start()
        {
            CheckMaterialState();
            CheckRenderPipeline();
        }

        void CheckRenderPipeline()
        {
            Debug.Log("=== Render Pipeline Check ===");

            // ���݂̃����_�[�p�C�v���C�����擾
            var currentPipeline = GraphicsSettings.currentRenderPipeline;
            if (currentPipeline != null)
            {
                Debug.Log($"Current Pipeline: {currentPipeline.name} (Type: {currentPipeline.GetType().Name})");

                // URP���ǂ����m�F
                if (currentPipeline is UniversalRenderPipelineAsset urpAsset)
                {
                    Debug.Log("URP is active");

                    // ���t���N�V�����œ����f�[�^���擾�i�o�[�W�����݊��̂��߁j
                    var fields = urpAsset.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        if (field.Name.Contains("renderer") || field.Name.Contains("Renderer"))
                        {
                            Debug.Log($"Found field: {field.Name}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("No Render Pipeline Asset found!");
            }
        }

        void CheckMaterialState()
        {
            if (timeStopMaterial == null)
            {
                Debug.LogError("TimeStop Material is not assigned!");
                return;
            }

            Debug.Log("=== Material State ===");
            Debug.Log($"Material: {timeStopMaterial.name}");
            Debug.Log($"Shader: {timeStopMaterial.shader.name}");

            // �S�v���p�e�B�����X�g
            Debug.Log("=== All Material Properties ===");
            var shader = timeStopMaterial.shader;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);

            for (int i = 0; i < propertyCount; i++)
            {
                string propName = ShaderUtil.GetPropertyName(shader, i);
                var propType = ShaderUtil.GetPropertyType(shader, i);

                switch (propType)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        if (timeStopMaterial.HasProperty(propName))
                        {
                            Color color = timeStopMaterial.GetColor(propName);
                            Debug.Log($"  {propName} (Color): {color}");
                        }
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                    case ShaderUtil.ShaderPropertyType.Range:
                        if (timeStopMaterial.HasProperty(propName))
                        {
                            float value = timeStopMaterial.GetFloat(propName);
                            Debug.Log($"  {propName} (Float): {value}");
                        }
                        break;
                }
            }

            // Render Queue ���m�F
            Debug.Log($"Render Queue: {timeStopMaterial.renderQueue}");
        }

        void Update()
        {
            // R�L�[�ŐF�������ύX���ăe�X�g
            if (Input.GetKeyDown(KeyCode.Q) && timeStopMaterial != null)
            {
                Debug.Log("=== Force Color Change Test ===");
                Debug.Log("Setting extreme values to test if shader responds...");

                // �ɒ[�Ȓl�ɕύX
                timeStopMaterial.SetColor("_TintColor", new Color(1f, 0f, 0f, 1f)); // �^����
                timeStopMaterial.SetColor("_VignetteColor", new Color(0f, 1f, 0f, 1f)); // �^��
                timeStopMaterial.SetFloat("_DesaturateAmount", 1f); // ���S���m�N��
                timeStopMaterial.SetFloat("_VignetteStrength", 1f); // �r�l�b�g�ő�
                timeStopMaterial.SetFloat("_ScanLineAlpha", 0.5f); // �X�L�������C���ő�
                timeStopMaterial.SetFloat("_Contrast", 1.5f); // �R���g���X�g�ő�
                timeStopMaterial.SetFloat("_Brightness", 0.3f); // ���邭

                Debug.Log("�� If screen doesn't turn red/green, the shader might not be applied correctly");
                Debug.Log($"New _TintColor: {timeStopMaterial.GetColor("_TintColor")}");
                Debug.Log($"New _VignetteColor: {timeStopMaterial.GetColor("_VignetteColor")}");
            }

            // T�L�[�Ō��ɖ߂�
            if (Input.GetKeyDown(KeyCode.T) && timeStopMaterial != null)
            {
                Debug.Log("=== Reset to Default ===");

                timeStopMaterial.SetColor("_TintColor", new Color(0.8f, 0.9f, 1f, 1f));
                timeStopMaterial.SetColor("_VignetteColor", new Color(0.3f, 0.4f, 0.5f, 1f));
                timeStopMaterial.SetFloat("_DesaturateAmount", 0.2f);
                timeStopMaterial.SetFloat("_VignetteStrength", 0.2f);
                timeStopMaterial.SetFloat("_ScanLineAlpha", 0.03f);
                timeStopMaterial.SetFloat("_Contrast", 1.05f);
                timeStopMaterial.SetFloat("_Brightness", -0.02f);

                Debug.Log("Reset to default colors");
            }

            // G�L�[��_Intensity���m�F
            if (Input.GetKeyDown(KeyCode.G) && timeStopMaterial != null)
            {
                float intensity = timeStopMaterial.GetFloat("_Intensity");
                Debug.Log($"Current _Intensity: {intensity}");

                // �����I��0.5�ɐݒ肵�ăe�X�g
                timeStopMaterial.SetFloat("_Intensity", 0.5f);
                Debug.Log("Set _Intensity to 0.5 for testing");
            }
        }
    }
}