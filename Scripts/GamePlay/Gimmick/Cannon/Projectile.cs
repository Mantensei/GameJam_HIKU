using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// �C�ӂ�GameObject��e�Ƃ��ē��삳����ėp�R���|�[�l���g
    /// ���ˎ��ɓ��I�ɃA�^�b�`����邱�Ƃ�z��
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public Vector2 Direction { get; private set; } = Vector2.right;

        //[field: SerializeField] public float LifeTime { get; private set; } = 0f;
        private float remainingLifeTime;

        [field: SerializeField] public LayerMask CollisionLayers { get; private set; } = -1;
        [field: SerializeField] public bool DestroyOnCollision { get; private set; } = false;

        // �������
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)] 
        public Rigidbody2D rb2d { get; private set; }

        // �C�x���g
        public event System.Action<Projectile> onLifetimeExpired;

        private void Start()
        {
            SetupPhysics();
            StartMovement();
        }

        void Update()
        {
            UpdateLifetime();
        }

        /// <summary>
        /// �e�̏������i��C����Ăяo�����j
        /// </summary>
        public void Initialize(CannonLauncher director, float speed, Vector2 direction)
        {
            Speed = speed;
            Direction = direction.normalized;
            //LifeTime = lifeTime;
            //remainingLifeTime = lifeTime;
        }

        /// <summary>
        /// �����ݒ�̃Z�b�g�A�b�v
        /// </summary>
        private void SetupPhysics()
        {
            //// �d�͐ݒ�
            //rb2d.gravityScale = UseGravity ? 1f : 0f;

            // ��]���Œ�i�K�v�ɉ����āj
            rb2d.freezeRotation = true;
        }

        /// <summary>
        /// �ړ��J�n
        /// </summary>
        private void StartMovement()
        {
            if (rb2d == null) return;

            rb2d.velocity = Direction * Speed;
        }

        /// <summary>
        /// �������Ԃ̍X�V
        /// </summary>
        private void UpdateLifetime()
        {
            //if (LifeTime <= 0f)
            //{
            //    remainingLifeTime -= Time.deltaTime;

            //    if (remainingLifeTime <= 0f)
            //    {
            //        HandleLifetimeExpired();
            //    }
            //}
        }

        /// <summary>
        /// �������ԏI�����̏���
        /// </summary>
        private void HandleLifetimeExpired()
        {
            onLifetimeExpired?.Invoke(this);

            Destroy(gameObject);
        }

        /// <summary>
        /// �Փˏ���
        /// </summary>
        void OnCollisionEnter2D(Collision2D collision)
        {
            // ���C���[�}�X�N�`�F�b�N
            if ((CollisionLayers.value & (1 << collision.gameObject.layer)) == 0)
                return;

            // �Փˎ��j��
            if (DestroyOnCollision)
            {
                Destroy(gameObject);
            }
        }
    }
}