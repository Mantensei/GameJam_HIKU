using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// 任意のGameObjectを弾として動作させる汎用コンポーネント
    /// 発射時に動的にアタッチされることを想定
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public Vector2 Direction { get; private set; } = Vector2.right;

        //[field: SerializeField] public float LifeTime { get; private set; } = 0f;
        private float remainingLifeTime;

        [field: SerializeField] public LayerMask CollisionLayers { get; private set; } = -1;
        [field: SerializeField] public bool DestroyOnCollision { get; private set; } = false;

        // 内部状態
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)] 
        public Rigidbody2D rb2d { get; private set; }

        // イベント
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
        /// 弾の初期化（大砲から呼び出される）
        /// </summary>
        public void Initialize(CannonLauncher director, float speed, Vector2 direction)
        {
            Speed = speed;
            Direction = direction.normalized;
            //LifeTime = lifeTime;
            //remainingLifeTime = lifeTime;
        }

        /// <summary>
        /// 物理設定のセットアップ
        /// </summary>
        private void SetupPhysics()
        {
            //// 重力設定
            //rb2d.gravityScale = UseGravity ? 1f : 0f;

            // 回転を固定（必要に応じて）
            rb2d.freezeRotation = true;
        }

        /// <summary>
        /// 移動開始
        /// </summary>
        private void StartMovement()
        {
            if (rb2d == null) return;

            rb2d.velocity = Direction * Speed;
        }

        /// <summary>
        /// 生存時間の更新
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
        /// 生存時間終了時の処理
        /// </summary>
        private void HandleLifetimeExpired()
        {
            onLifetimeExpired?.Invoke(this);

            Destroy(gameObject);
        }

        /// <summary>
        /// 衝突処理
        /// </summary>
        void OnCollisionEnter2D(Collision2D collision)
        {
            // レイヤーマスクチェック
            if ((CollisionLayers.value & (1 << collision.gameObject.layer)) == 0)
                return;

            // 衝突時破壊
            if (DestroyOnCollision)
            {
                Destroy(gameObject);
            }
        }
    }
}