using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>タイマー連動で動く足場（乗客運搬機能付き）</summary>
    public class MovingPlatform : MonoBehaviour
    {
        [field: SerializeField] public float MoveDegree { get; private set; } = 0f;
        [field: SerializeField] public float MoveDistance { get; private set; } = 5f;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2f;
        [field: SerializeField] public bool StartFromCenter { get; private set; } = true;
        [field: SerializeField] public Ease EaseType { get; private set; } = Ease.InOutSine;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _moveDirection;
        private Vector3 _lastPlatformPosition;
        private float _lastUpdateTime = -1f;

        // 乗客管理
        private List<IRb2d> _passengers = new List<IRb2d>();

        void Start()
        {
            InitializePlatform();
            _lastPlatformPosition = transform.position;
        }

        void Update()
        {
            UpdatePosition();
        }

        void LateUpdate()
        {
            // 他のスクリプトの後で実行
            CheckPassengers();
        }

        /// <summary>足場の初期化</summary>
        private void InitializePlatform()
        {
            // 移動方向を角度から計算
            float radians = MoveDegree * Mathf.Deg2Rad;
            _moveDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);

            // 開始位置と終了位置を設定
            if (StartFromCenter)
            {
                // 現在位置を中心として前後に移動
                _startPosition = transform.position - _moveDirection * (MoveDistance * 0.5f);
                _endPosition = transform.position + _moveDirection * (MoveDistance * 0.5f);
            }
            else
            {
                // 現在位置から指定方向に移動
                _startPosition = transform.position;
                _endPosition = transform.position + _moveDirection * MoveDistance;
            }
        }

        /// <summary>タイマー連動の位置更新</summary>
        private void UpdatePosition()
        {
            // UIHub.Instance.Timer から現在時刻を取得
            var timer = UIHub.Instance?.Timer;
            if (timer == null) return;

            float currentTime = timer.CurrentTime;

            // タイマーが止まっている場合は位置更新しない
            if (Mathf.Approximately(currentTime, _lastUpdateTime)) return;
            _lastUpdateTime = currentTime;

            // 移動の周期を計算（往復時間 = 距離 / 速度 * 2）
            float cycleDuration = (MoveDistance / MoveSpeed) * 2f;
            if (cycleDuration <= 0f) return;

            // 現在の周期内時間を取得（0～cycleDuration）
            float cycleTime = currentTime % cycleDuration;

            // 0～1の進行度に変換（往復を考慮）
            float progress;
            if (cycleTime <= cycleDuration * 0.5f)
            {
                // 前半：開始→終了
                progress = (cycleTime / (cycleDuration * 0.5f));
            }
            else
            {
                // 後半：終了→開始
                progress = 1f - ((cycleTime - cycleDuration * 0.5f) / (cycleDuration * 0.5f));
            }

            // Easing適用
            float easedProgress = DOVirtual.EasedValue(0f, 1f, progress, EaseType);

            // 新しい位置を計算
            Vector3 newPosition = Vector3.Lerp(_startPosition, _endPosition, easedProgress);
            Vector3 deltaMove = newPosition - transform.position;

            // 足場を移動
            transform.position = newPosition;

            // 乗客も一緒に移動
            if (_passengers.Count > 0)
            {
                MovePlatformPassengers(deltaMove);
            }

            _lastPlatformPosition = newPosition;
        }

        /// <summary>乗客を一緒に移動させる</summary>
        private void MovePlatformPassengers(Vector3 deltaMove)
        {
            foreach (var passenger in _passengers)
            {
                if (passenger?.rb2d != null)
                {
                    // 直接 transform.position を変更してテスト
                    passenger.transform.position += deltaMove;

                    // 次フレームでRigidbody2Dに反映
                    Vector2 newRbPosition = passenger.transform.position;
                    passenger.rb2d.position = newRbPosition;
                }
            }
        }

        /// <summary>ContactFilter2Dで乗客をチェック</summary>
        private void CheckPassengers()
        {
            // 上向きの接触のみ検出（上向き±20度の範囲）
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetNormalAngle(70f, 110f);
            filter.useNormalAngle = true;
            filter.useTriggers = false; // Triggerは除外

            Collider2D[] results = new Collider2D[10];
            int count = GetComponent<Collider2D>().OverlapCollider(filter, results);

            // 現在の乗客リストをクリア
            int oldPassengerCount = _passengers.Count;
            _passengers.Clear();

            // 接触中のIRb2dを乗客に追加
            for (int i = 0; i < count; i++)
            {
                if (results[i] != null && results[i].TryGetComponent<IRb2d>(out var passenger))
                {
                    _passengers.Add(passenger);
                }
            }
        }

        /// <summary>エディタでの可視化</summary>
        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            // 初期化されていない場合は仮計算
            Vector3 startPos, endPos;
            float radians = MoveDegree * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);

            if (StartFromCenter)
            {
                startPos = transform.position - direction * (MoveDistance * 0.5f);
                endPos = transform.position + direction * (MoveDistance * 0.5f);
            }
            else
            {
                startPos = transform.position;
                endPos = transform.position + direction * MoveDistance;
            }

            // 移動範囲の描画
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, endPos);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos, 0.2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(endPos, 0.2f);

            // 方向矢印
            Gizmos.color = Color.yellow;
            Vector3 arrowStart = startPos + (endPos - startPos) * 0.3f;
            Vector3 arrowEnd = startPos + (endPos - startPos) * 0.7f;
            Gizmos.DrawLine(arrowStart, arrowEnd);
        }
    }
}