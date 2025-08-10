using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GameJam_HIKU
{
    public class CannonLauncher : MonoBehaviour
    {
        // [既存のフィールドはそのまま]
        [field: SerializeField] public GameObject[] ProjectilePrefabs { get; private set; }
        [field: SerializeField] public int CurrentProjectileIndex { get; private set; } = 0;
        [field: SerializeField] public int MaxCount { get; private set; } = 32;

        [field: SerializeField] public Transform LaunchPoint { get; private set; }
        [field: SerializeField] public float LaunchSpeed { get; private set; } = 10f;
        [field: SerializeField] public Vector2 LaunchDirection { get; private set; } = Vector2.right;
        [field: SerializeField] public bool UseOriginalRotation { get; private set; }

        public bool AutoFire => FireInterval > 0f;
        [field: SerializeField] public float FireInterval { get; private set; } = 1f;
        [field: SerializeField] public bool SetToChildren { get; private set; } = true;
        [field: SerializeField] public bool VisibleUpdate { get; private set; }
        [field: SerializeField] public bool FireOnAwake { get; private set; }

        // 発射済み弾の管理
        private List<GameObject> projectiles = new List<GameObject>();
        public int Count => projectiles.Count;

        // タイマー管理
        private float fireTimer = 0f;
        private Camera targetCamera;
        private bool isVisible;

        // イベント
        public event System.Action<CannonLauncher, GameObject> onFired;
        public event System.Action<CannonLauncher> onProjectileChanged;

        void Start()
        {
            if (LaunchPoint == null)
            {
                LaunchPoint = transform;
            }

            if(FireOnAwake)
            {
                TryFire();
            }
        }

        void Update()
        {
            CleanupDestroyedProjectiles();

            HandleInput();
            HandleAutoFire();
        }

        void OnBecameVisible()
        {
            isVisible = true;
        }

        void OnBecameInvisible()
        {
            isVisible = false;
        }

        private void HandleAutoFire()
        {
            if (!AutoFire) return;

            // VisibleUpdateがtrueの場合、画面外では更新しない
            if (VisibleUpdate && !isVisible) return;

            // タイマー更新
            fireTimer += Time.deltaTime;

            if (fireTimer >= FireInterval)
            {
                if (TryFire())
                {
                    fireTimer = 0f;
                }
            }
        }

        /// <summary>発射を試行（上限チェック付き）</summary>
        public bool TryFire()
        {
            // 上限チェック
            if (MaxCount > 0 && Count >= MaxCount)
            {
                return false;
            }

            Fire();
            return true;
        }

        public void Fire()
        {
            if (ProjectilePrefabs == null || ProjectilePrefabs.Length == 0)
            {
                Debug.LogWarning("ProjectilePrefabs が設定されていません");
                return;
            }

            if (CurrentProjectileIndex < 0 || CurrentProjectileIndex >= ProjectilePrefabs.Length)
            {
                Debug.LogWarning($"無効な ProjectileIndex: {CurrentProjectileIndex}");
                return;
            }

            GameObject prefab = ProjectilePrefabs[CurrentProjectileIndex];
            if (prefab == null)
            {
                Debug.LogWarning($"ProjectilePrefab[{CurrentProjectileIndex}] が null です");
                return;
            }

            // 発射方向の決定
            Vector2 fireDirection = GetFireDirection();

            // 回転の決定
            Quaternion rotation = UseOriginalRotation ? LaunchPoint.rotation : prefab.transform.rotation;

            // インスタンス生成
            GameObject instance = Instantiate(prefab, LaunchPoint.position, rotation);

            if (SetToChildren)
            {
                instance.transform.SetParent(transform);
            }

            // 発射済みリストに追加
            projectiles.Add(instance);

            // ProjectileComponent を動的追加
            Projectile projectile = instance.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = instance.AddComponent<Projectile>();
            }

            // 弾を初期化・発射
            projectile.Initialize(this, LaunchSpeed, fireDirection);
            instance.AddComponent<GetComponentAutoInitializer>();

            onFired?.Invoke(this, instance);
        }

        private Vector2 GetFireDirection()
        {
            Vector2 baseDirection;

            if (targetCamera != null)
            {
                Vector3 mouseWorldPos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                baseDirection = (mouseWorldPos - LaunchPoint.position).normalized;
            }
            else
            {
                baseDirection = LaunchDirection.normalized;
            }

            // UseOriginalRotationがtrueの場合、LaunchPointの回転を適用
            if (UseOriginalRotation)
            {
                float rotationAngle = LaunchPoint.eulerAngles.z * Mathf.Deg2Rad;
                Vector2 rotatedDirection = new Vector2(
                    baseDirection.x * Mathf.Cos(rotationAngle) - baseDirection.y * Mathf.Sin(rotationAngle),
                    baseDirection.x * Mathf.Sin(rotationAngle) + baseDirection.y * Mathf.Cos(rotationAngle)
                );
                return rotatedDirection.normalized;
            }

            return baseDirection;
        }

        /// <summary>破棄された弾をリストから除去</summary>
        private void CleanupDestroyedProjectiles()
        {
            projectiles.RemoveAll(p => !(p?.IsSafe() == true));
        }

        // [既存のメソッドはそのまま]
        private void HandleInput()
        {
            for (int i = 1; i <= Mathf.Min(ProjectilePrefabs.Length, 9); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    SetCurrentProjectile(i - 1);
                }
            }
        }

        public void SetCurrentProjectile(int index)
        {
            if (index < 0 || index >= ProjectilePrefabs.Length) return;

            CurrentProjectileIndex = index;
            onProjectileChanged?.Invoke(this);
        }

        public void NextProjectile()
        {
            if (ProjectilePrefabs.Length == 0) return;

            CurrentProjectileIndex = (CurrentProjectileIndex + 1) % ProjectilePrefabs.Length;
            onProjectileChanged?.Invoke(this);
        }

        public void PreviousProjectile()
        {
            if (ProjectilePrefabs.Length == 0) return;

            CurrentProjectileIndex = (CurrentProjectileIndex - 1 + ProjectilePrefabs.Length) % ProjectilePrefabs.Length;
            onProjectileChanged?.Invoke(this);
        }

        public GameObject GetCurrentProjectilePrefab()
        {
            if (ProjectilePrefabs == null || CurrentProjectileIndex < 0 || CurrentProjectileIndex >= ProjectilePrefabs.Length)
                return null;

            return ProjectilePrefabs[CurrentProjectileIndex];
        }
    }
}