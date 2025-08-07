using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>���E���̏����\�I�u�W�F�N�g���m�V�X�e��</summary>
    public class SubtractTargetDetector : MonoBehaviour
    {
        [field: SerializeField] public Camera TargetCamera { get; private set; }
        [field: SerializeField] public LayerMask DetectionLayers { get; private set; } = -1;
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

        /// <summary>�X�L���������̃^�[�Q�b�g���m</summary>
        public void UpdateTargets()
        {
            detectedTargets.Clear();

            // ��ʓ���RemovableObject���擾
            var screenTargets = RaycastTargetFinder.FindTargetsInScreenBounds(TargetCamera, DetectionLayers);

            foreach (var target in screenTargets)
            {
                var removable = target.GetComponent<RemovableObject>();
                if (removable != null)
                {
                    // �����`�F�b�N
                    if (RequireLineOfSight && !RaycastTargetFinder.HasLineOfSight(transform.position, target.transform.position, DetectionLayers))
                    {
                        continue;
                    }

                    detectedTargets.Add(removable);
                }
            }

            UpdateHoverTarget();
        }

        /// <summary>�}�E�X�J�[�\�����̃^�[�Q�b�g�X�V</summary>
        private void UpdateHoverTarget()
        {
            var hoverTarget = RaycastTargetFinder.GetTargetUnderCursor(TargetCamera, DetectionLayers);
            var removable = hoverTarget?.GetComponent<RemovableObject>();

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

        /// <summary>�^�[�Q�b�g���N���b�N�폜</summary>
        public bool TryRemoveHoverTarget()
        {
            if (currentHoverTarget == null) return false;

            var systemDamage = DamageInfoExtensions.CreateSystemDamage(gameObject);
            currentHoverTarget.TakeDamage(systemDamage);
            return true;
        }
    }

    /// <summary>���C�L���X�g�ɂ��^�[�Q�b�g����</summary>
    public static class RaycastTargetFinder
    {
        /// <summary>��ʋ��E���̃^�[�Q�b�g�擾</summary>
        public static List<GameObject> FindTargetsInScreenBounds(Camera camera, LayerMask layers)
        {
            var targets = new List<GameObject>();
            if (camera == null) return targets;

            // ��ʓ���Collider���擾
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

        /// <summary>�J�[�\�����̃^�[�Q�b�g�擾</summary>
        public static GameObject GetTargetUnderCursor(Camera camera, LayerMask layers)
        {
            if (camera == null) return null;

            Vector2 worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, layers);

            return hit.collider?.gameObject;
        }

        /// <summary>�����`�F�b�N</summary>
        public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacleLayers)
        {
            Vector2 direction = (to - from).normalized;
            float distance = Vector2.Distance(from, to);

            RaycastHit2D hit = Physics2D.Raycast(from, direction, distance, obstacleLayers);

            // �q�b�g�����I�u�W�F�N�g���^�[�Q�b�g���g�Ȃ王���͒ʂ��Ă���
            return hit.collider == null || Vector2.Distance(hit.point, to) < 0.1f;
        }

        /// <summary>�J�����̉�ʋ��E�擾</summary>
        private static Bounds GetScreenBounds(Camera camera)
        {
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            var topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));

            var center = (bottomLeft + topRight) * 0.5f;
            var size = topRight - bottomLeft;

            return new Bounds(center, size);
        }

        /// <summary>�J�������E������</summary>
        private static bool IsInCameraView(Camera camera, Vector3 worldPosition)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
            return screenPoint.x >= 0 && screenPoint.x <= camera.pixelWidth &&
                   screenPoint.y >= 0 && screenPoint.y <= camera.pixelHeight &&
                   screenPoint.z > 0;
        }
    }
}