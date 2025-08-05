using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// �C�ӂ�GameObject��e�Ƃ��Ĕ��˂����C�V�X�e��
    /// ���ˎ���ProjectileComponent�𓮓I�ɃA�^�b�`
    /// </summary>
    public class CannonLauncher : MonoBehaviour
    {
        //[Header("Projectile Settings")]
        [field: SerializeField] public GameObject[] ProjectilePrefabs { get; private set; }
        [field: SerializeField] public int CurrentProjectileIndex { get; private set; } = 0;

        //[Header("Launch Settings")]
        [field: SerializeField] public Transform LaunchPoint { get; private set; }
        [field: SerializeField] public float LaunchSpeed { get; private set; } = 10f;
        [field: SerializeField] public Vector2 LaunchDirection { get; private set; } = Vector2.right;
        [field: SerializeField] public bool UseGravityForProjectiles { get; private set; } = false;
        [field: SerializeField] public float ProjectileLifeTime { get; private set; } = 5f;

        //[Header("Auto Fire Settings")]
        public bool AutoFire => FireInterval > 0f;
        [field: SerializeField] public float FireInterval { get; private set; } = 1f;

        //[Header("Input Settings")]
        //[field: SerializeField] public KeyCode FireKey { get; private set; } = KeyCode.Space;
        [field: SerializeField] public bool UseMouseDirection { get; private set; } = false;

        //[Header("Projectile Physics")]
        [field: SerializeField] public LayerMask CollisionLayers { get; private set; } = -1;
        [field: SerializeField] public bool DestroyOnCollision { get; private set; } = false;
        [field: SerializeField] public bool DamageOnCollision { get; private set; } = false;
        [field: SerializeField] public DamageInfo CollisionDamage { get; private set; }

        private float nextFireTime = 0f;
        private Camera targetCamera;

        // �C�x���g
        public event System.Action<CannonLauncher, GameObject> onFired;
        public event System.Action<CannonLauncher> onProjectileChanged;

        void Start()
        {
            // ���˒n�_���w�肳��Ă��Ȃ��ꍇ�͎��g���g�p
            if (LaunchPoint == null)
            {
                LaunchPoint = transform;
            }

            // �}�E�X�����g�p���̃J�����擾
            if (UseMouseDirection)
            {
                targetCamera = Camera.main ?? FindObjectOfType<Camera>();
            }
        }

        void Update()
        {
            HandleInput();
            HandleAutoFire();
        }

        private void HandleInput()
        {
            //if (Input.GetKeyDown(FireKey))
            //{
            //    Fire();
            //}

            // �e�̐؂�ւ��i�����L�[�j
            for (int i = 1; i <= Mathf.Min(ProjectilePrefabs.Length, 9); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    SetCurrentProjectile(i - 1);
                }
            }
        }

        private void HandleAutoFire()
        {
            if (!AutoFire) return;

            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + FireInterval;
            }
        }

        public void Fire()
        {
            if (ProjectilePrefabs == null || ProjectilePrefabs.Length == 0)
            {
                Debug.LogWarning("ProjectilePrefabs ���ݒ肳��Ă��܂���");
                return;
            }

            if (CurrentProjectileIndex < 0 || CurrentProjectileIndex >= ProjectilePrefabs.Length)
            {
                Debug.LogWarning($"������ ProjectileIndex: {CurrentProjectileIndex}");
                return;
            }

            GameObject prefab = ProjectilePrefabs[CurrentProjectileIndex];
            if (prefab == null)
            {
                Debug.LogWarning($"ProjectilePrefab[{CurrentProjectileIndex}] �� null �ł�");
                return;
            }

            // ���˕����̌���
            Vector2 fireDirection = GetFireDirection();

            // �C���X�^���X����
            GameObject instance = Instantiate(prefab, LaunchPoint.position, LaunchPoint.rotation);

            // ProjectileComponent �𓮓I�ǉ�
            Projectile projectile = instance.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = instance.AddComponent<Projectile>();
            }

            // �e���������E����
            projectile.Initialize(LaunchSpeed, fireDirection, ProjectileLifeTime);
            instance.AddComponent<GetComponentAutoInitializer>();

            onFired?.Invoke(this, instance);
        }

        private Vector2 GetFireDirection()
        {
            Vector2 baseDirection;

            if (UseMouseDirection && targetCamera != null)
            {
                Vector3 mouseWorldPos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                baseDirection = (mouseWorldPos - LaunchPoint.position).normalized;
            }
            else
            {
                baseDirection = LaunchDirection.normalized;
            }

            // Transform.Rotation�ɉ����Ĕ��˕�������]
            float rotationAngle = LaunchPoint.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 rotatedDirection = new Vector2(
                baseDirection.x * Mathf.Cos(rotationAngle) - baseDirection.y * Mathf.Sin(rotationAngle),
                baseDirection.x * Mathf.Sin(rotationAngle) + baseDirection.y * Mathf.Cos(rotationAngle)
            );

            return rotatedDirection.normalized;
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