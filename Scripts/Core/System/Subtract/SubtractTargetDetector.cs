using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using UnityEngine.EventSystems;

namespace GameJam_HIKU
{
    /// <summary>視界内の消去可能オブジェクト検知システム</summary>
    public class SubtractTargetDetector : MonoBehaviour
    {
        [field: SerializeField] public Camera TargetCamera { get; private set; }
        [field: SerializeField] public LayerMask DetectionLayers { get; private set; } = -1;
        [field: SerializeField] public LayerMask WallLayers { get; private set; } = 1 << 3; // Default レイヤー等
        [field: SerializeField] public float DetectionRange { get; private set; } = 10f;
        [field: SerializeField] public bool RequireLineOfSight { get; private set; } = true;

        private List<RemovableObject> detectedTargets = new List<RemovableObject>();
        private RemovableObject currentHoverTarget = null;

        public event System.Action<RemovableObject> OnTargetEnter;
        public event System.Action<RemovableObject> OnTargetExit;
        public event System.Action<RemovableObject> OnTargetHover;

        public IReadOnlyList<RemovableObject> DetectedTargets => detectedTargets;
        public RemovableObject CurrentHoverTarget => currentHoverTarget;

        void Start()
        {
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main ?? FindObjectOfType<Camera>();
            }
        }

        /// <summary>スキル発動中のターゲット検知</summary>
        public void UpdateTargets()
        {
            detectedTargets.Clear();

            // 1. 通常のゲームオブジェクト検知（既存処理）
            var screenTargets = RaycastTargetFinder.FindTargetsInScreenBounds(TargetCamera, DetectionLayers);
            foreach (var target in screenTargets)
            {
                var removable = target.GetComponent<RemovableObject>();
                if (removable != null)
                {
                    if (RequireLineOfSight && !RaycastTargetFinder.HasLineOfSight(transform.position, target.transform.position, WallLayers))
                    {
                        continue;
                    }
                    detectedTargets.Add(removable);
                }
            }

            // 2. UI要素の検知（新規追加）
            DetectUITargets();

            UpdateHoverTarget();
        }

        /// <summary>UI要素の削除可能オブジェクトを検知</summary>
        private void DetectUITargets()
        {
            // GraphicRaycasterを使用してUI要素を検知
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (eventSystem == null) return;

            var pointerEventData = new UnityEngine.EventSystems.PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<UnityEngine.EventSystems.RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, raycastResults);

            foreach (var result in raycastResults)
            {
                var removable = result.gameObject.GetComponent<RemovableObject>();
                if (removable != null && !detectedTargets.Contains(removable))
                {
                    detectedTargets.Add(removable);
                }
            }
        }

        /// <summary>マウスカーソル下のターゲット更新</summary>
        private void UpdateHoverTarget()
        {
            RemovableObject removable = null;

            // 1. まずUI要素をチェック
            var uiTarget = GetUITargetUnderCursor();
            if (uiTarget != null)
            {
                removable = uiTarget;
            }
            else
            {
                // 2. 物理オブジェクトを全てチェック（RaycastAllを使用）
                removable = GetPhysicsTargetUnderCursor();
            }

            // 検出したオブジェクトがdetectedTargetsに含まれているか確認
            if (removable != null && detectedTargets.Contains(removable))
            {
                if (currentHoverTarget != removable)
                {
                    if (currentHoverTarget != null)
                    {
                        OnTargetExit?.Invoke(currentHoverTarget);
                    }

                    currentHoverTarget = removable;
                    OnTargetEnter?.Invoke(currentHoverTarget);
                }

                OnTargetHover?.Invoke(currentHoverTarget);
            }
            else
            {
                if (currentHoverTarget != null)
                {
                    OnTargetExit?.Invoke(currentHoverTarget);
                    currentHoverTarget = null;
                }
            }
        }

        /// <summary>物理オブジェクトの検出（親子関係も考慮）</summary>
        private RemovableObject GetPhysicsTargetUnderCursor()
        {
            if (TargetCamera == null) return null;

            Vector2 worldPos = TargetCamera.ScreenToWorldPoint(Input.mousePosition);

            // 全てのヒットを取得
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, 0f, DetectionLayers);

            // RemovableObjectを持つものを優先的に返す
            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                // 1. 自身にRemovableObjectがあるか
                var removable = hit.collider.GetComponent<RemovableObject>();
                if (removable != null) return removable;

                // 2. 親にRemovableObjectがあるか
                removable = hit.collider.GetComponentInParent<RemovableObject>();
                if (removable != null) return removable;

                // 3. 子にRemovableObjectがあるか（オプション）
                removable = hit.collider.GetComponentInChildren<RemovableObject>();
                if (removable != null) return removable;
            }

            return null;
        }

        /// <summary>カーソル下のUIターゲットを取得</summary>
        private RemovableObject GetUITargetUnderCursor()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return null;

            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                var removable = result.gameObject.GetComponent<RemovableObject>();
                if (removable != null)
                {
                    return removable;
                }
            }

            return null;
        }

        /// <summary>ターゲットをクリック削除</summary>
        public bool TryRemoveHoverTarget()
        {
            if (currentHoverTarget == null) return false;

            var systemDamage = DamageInfoExtensions.CreateSystemDamage(gameObject);
            currentHoverTarget.TakeDamage(systemDamage);
            return true;
        }
    }

    /// <summary>レイキャストによるターゲット検索</summary>
    public static class RaycastTargetFinder
    {
        /// <summary>画面境界内のターゲット取得</summary>
        public static List<GameObject> FindTargetsInScreenBounds(Camera camera, LayerMask layers)
        {
            var targets = new List<GameObject>();
            if (camera == null) return targets;

            // 画面内のColliderを取得
            var screenBounds = GetScreenBounds(camera);
            var colliders = Physics2D.OverlapAreaAll(screenBounds.min, screenBounds.max, layers);

            foreach (var collider in colliders)
            {
                if (IsInCameraView(camera, collider.transform.position))
                {
                    targets.Add(collider.gameObject);
                }
            }

            return targets;
        }

        /// <summary>カーソル下のターゲット取得（親子関係考慮）</summary>
        public static GameObject GetTargetUnderCursor(Camera camera, LayerMask layers)
        {
            if (camera == null) return null;

            Vector2 worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, 0f, layers);

            // RemovableObjectを持つGameObjectを優先
            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                // RemovableObjectを持つオブジェクトを探す
                if (hit.collider.GetComponent<RemovableObject>() != null)
                    return hit.collider.gameObject;

                // 親にRemovableObjectがある場合はその親を返す
                var parentRemovable = hit.collider.GetComponentInParent<RemovableObject>();
                if (parentRemovable != null)
                    return parentRemovable.gameObject;
            }

            // RemovableObjectが見つからない場合は最初のヒットを返す
            return hits.Length > 0 ? hits[0].collider.gameObject : null;
        }

        /// <summary>視線チェック</summary>
        public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacleLayers)
        {
            Vector2 direction = (to - from).normalized;
            float distance = Vector2.Distance(from, to);

            RaycastHit2D hit = Physics2D.Raycast(from, direction, distance, obstacleLayers);
            // ヒットしたオブジェクトがターゲット自身なら視線は通っている
            bool result = hit.collider == null || Vector2.Distance(hit.point, to) < 0.1f;

#if UNITY_EDITOR
            var color = result ? Color.green : Color.red;
            Debug.DrawRay(from, direction * distance, color, 1f);
#endif

            return result;
        }

        /// <summary>カメラの画面境界取得</summary>
        private static Bounds GetScreenBounds(Camera camera)
        {
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            var topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));

            var center = (bottomLeft + topRight) * 0.5f;
            var size = topRight - bottomLeft;

            return new Bounds(center, size);
        }

        /// <summary>カメラ視界内判定</summary>
        private static bool IsInCameraView(Camera camera, Vector3 worldPosition)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
            return screenPoint.x >= 0 && screenPoint.x <= camera.pixelWidth &&
                   screenPoint.y >= 0 && screenPoint.y <= camera.pixelHeight &&
                   screenPoint.z > 0;
        }
    }
}