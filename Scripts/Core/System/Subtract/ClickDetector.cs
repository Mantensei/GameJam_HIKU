using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// UI�EGameObject�����ɑΉ���������N���b�N���m�R���|�[�l���g
    /// ���m��A�_���[�W�V�X�e����ʂ��č폜���������s
    /// </summary>
    public class ClickDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private LayerMask gameObjectLayers = -1;
        [SerializeField] private bool detectUI = true;
        [SerializeField] private bool detectGameObjects = true;

        [Header("System Damage Settings")]
        [SerializeField] private GameObject attacker;

        private Camera targetCamera;

        void Start()
        {
            // �J�����̎擾�i�w�肳��Ă��Ȃ��ꍇ�̓��C���J�����j
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }

            // attacker���w�肳��Ă��Ȃ��ꍇ�͎��g��ݒ�
            if (attacker == null)
            {
                attacker = gameObject;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // �E�N���b�N
            {
                HandleClick();
            }
        }

        /// <summary>
        /// �N���b�N�����̃��C���֐�
        /// </summary>
        private void HandleClick()
        {
            Vector2 mousePosition = Input.mousePosition;

            // UI�D��Ń`�F�b�N
            if (detectUI && IsPointerOverUI())
            {
                Debug.Log("a");
                HandleUIClick();
                return;
            }

            // GameObject �̃`�F�b�N
            if (detectGameObjects)
            {
                Debug.Log("b");
                HandleGameObjectClick(mousePosition);
            }
        }

        /// <summary>
        /// UI���N���b�N����Ă��邩�`�F�b�N
        /// </summary>
        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// UI�N���b�N����
        /// </summary>
        private void HandleUIClick()
        {
            Debug.Log(EventSystem.current);
            if (EventSystem.current == null) return;

            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            Debug.Log(0);

            foreach (var result in results)
            {
                Debug.Log(123123);
                // RemovableObject��D��I�Ƀ`�F�b�N
                var removable = result.gameObject.GetComponent<RemovableObject>();
                if (removable != null)
                {
                Debug.Log(1);
                    var systemDamage = DamageInfoExtensions.CreateSystemDamage(attacker);
                    removable.TakeDamage(systemDamage);
                }
            }
        }

        /// <summary>
        /// GameObject �N���b�N����
        /// </summary>
        private void HandleGameObjectClick(Vector2 screenPosition)
        {
            if (targetCamera == null) return;

            Vector2 worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, gameObjectLayers);

            if (hit.collider != null)
            {
                // RemovableObject��D��I�Ƀ`�F�b�N
                var removable = hit.collider.GetComponent<RemovableObject>();
                if (removable != null)
                {
                    var systemDamage = DamageInfoExtensions.CreateSystemDamage(attacker);
                    removable.TakeDamage(systemDamage);
                }
            }
        }
    }
}