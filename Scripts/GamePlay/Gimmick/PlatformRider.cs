using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>MovingPlatform�ɏ��I�u�W�F�N�g�p�R���|�[�l���g</summary>
    public class PlatformRider : MonoBehaviour
    {
        private MovingPlatform _currentPlatform;
        private Vector3 _lastPlatformPosition;
        private bool _isOnPlatform = false;

        void Update()
        {
            if (_isOnPlatform && _currentPlatform != null)
            {
                // ����̈ړ��ʂ����̃I�u�W�F�N�g�ɂ��K�p
                Vector3 platformDelta = _currentPlatform.transform.position - _lastPlatformPosition;
                transform.position += platformDelta;
                _lastPlatformPosition = _currentPlatform.transform.position;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<MovingPlatform>(out var platform))
            {
                // ����̏�ɏ�����ꍇ�̂ݒǏ]�J�n
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

        /// <summary>����ɏ��n�߂�</summary>
        private void StartRiding(MovingPlatform platform)
        {
            _currentPlatform = platform;
            _lastPlatformPosition = platform.transform.position;
            _isOnPlatform = true;
        }

        /// <summary>���ꂩ��~���</summary>
        private void StopRiding()
        {
            _currentPlatform = null;
            _isOnPlatform = false;
        }

        /// <summary>����̏�ɏ���Ă��邩�`�F�b�N</summary>
        private bool IsOnTopOfPlatform(Collision2D collision)
        {
            // �Փ˂����ڐG�_������̏㕔���ǂ����𔻒�
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f) // ������̖@��
                {
                    return true;
                }
            }
            return false;
        }
    }
}