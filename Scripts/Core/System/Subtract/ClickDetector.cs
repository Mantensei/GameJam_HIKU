using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// UI・GameObject両方に対応した統一クリック検知コンポーネント
    /// 検知後、ダメージシステムを通じて削除処理を実行
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
            // カメラの取得（指定されていない場合はメインカメラ）
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }

            // attackerが指定されていない場合は自身を設定
            if (attacker == null)
            {
                attacker = gameObject;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 右クリック
            {
                HandleClick();
            }
        }

        /// <summary>
        /// クリック処理のメイン関数
        /// </summary>
        private void HandleClick()
        {
            Vector2 mousePosition = Input.mousePosition;

            // UI優先でチェック
            if (detectUI && IsPointerOverUI())
            {
                Debug.Log("a");
                HandleUIClick();
                return;
            }

            // GameObject のチェック
            if (detectGameObjects)
            {
                Debug.Log("b");
                HandleGameObjectClick(mousePosition);
            }
        }

        /// <summary>
        /// UIがクリックされているかチェック
        /// </summary>
        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// UIクリック処理
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
                // RemovableObjectを優先的にチェック
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
        /// GameObject クリック処理
        /// </summary>
        private void HandleGameObjectClick(Vector2 screenPosition)
        {
            if (targetCamera == null) return;

            Vector2 worldPosition = targetCamera.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, gameObjectLayers);

            if (hit.collider != null)
            {
                // RemovableObjectを優先的にチェック
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