using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameJam_HIKU
{
    /// <summary>タイマー連動で動く足場</summary>
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
        private float _lastUpdateTime = -1f;
        private bool _isInitialized = false;

        void Start()
        {
            InitializePlatform();
        }

        void Update()
        {
            UpdatePosition();
        }

        /// <summary>足場の初期化</summary>
        private void InitializePlatform()
        {
            if (_isInitialized) return;

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

            _isInitialized = true;
        }

        /// <summary>タイマー連動の位置更新</summary>
        private void UpdatePosition()
        {
            if (!_isInitialized) return;

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

            // 位置を更新
            transform.position = Vector3.Lerp(_startPosition, _endPosition, easedProgress);
        }

        /// <summary>エディタでの可視化</summary>
        void OnDrawGizmosSelected()
        {
            if (!_isInitialized && Application.isPlaying) return;

            // 初期化されていない場合は仮計算
            Vector3 startPos, endPos;
            if (!_isInitialized)
            {
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
            }
            else
            {
                startPos = _startPosition;
                endPos = _endPosition;
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