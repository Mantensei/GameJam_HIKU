using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>MovingPlatformに乗るオブジェクト用コンポーネント</summary>
    public class PlatformRider : MonoBehaviour
    {
        private MovingPlatform _currentPlatform;
        private Vector3 _lastPlatformPosition;
        private bool _isOnPlatform = false;

        void Update()
        {
            if (_isOnPlatform && _currentPlatform != null)
            {
                // 足場の移動量をこのオブジェクトにも適用
                Vector3 platformDelta = _currentPlatform.transform.position - _lastPlatformPosition;
                transform.position += platformDelta;
                _lastPlatformPosition = _currentPlatform.transform.position;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<MovingPlatform>(out var platform))
            {
                // 足場の上に乗った場合のみ追従開始
                if (IsOnTopOfPlatform(collision))
                {
                    StartRiding(platform);
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<MovingPlatform>(out var platform))
            {
                if (_currentPlatform == platform)
                {
                    StopRiding();
                }
            }
        }

        /// <summary>足場に乗り始める</summary>
        private void StartRiding(MovingPlatform platform)
        {
            _currentPlatform = platform;
            _lastPlatformPosition = platform.transform.position;
            _isOnPlatform = true;
        }

        /// <summary>足場から降りる</summary>
        private void StopRiding()
        {
            _currentPlatform = null;
            _isOnPlatform = false;
        }

        /// <summary>足場の上に乗っているかチェック</summary>
        private bool IsOnTopOfPlatform(Collision2D collision)
        {
            // 衝突した接触点が足場の上部かどうかを判定
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f) // 上向きの法線
                {
                    return true;
                }
            }
            return false;
        }
    }
}