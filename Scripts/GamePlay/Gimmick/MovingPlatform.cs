using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameJam_HIKU
{
    /// <summary>�^�C�}�[�A���œ�������</summary>
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

        /// <summary>����̏�����</summary>
        private void InitializePlatform()
        {
            if (_isInitialized) return;

            // �ړ��������p�x����v�Z
            float radians = MoveDegree * Mathf.Deg2Rad;
            _moveDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);

            // �J�n�ʒu�ƏI���ʒu��ݒ�
            if (StartFromCenter)
            {
                // ���݈ʒu�𒆐S�Ƃ��đO��Ɉړ�
                _startPosition = transform.position - _moveDirection * (MoveDistance * 0.5f);
                _endPosition = transform.position + _moveDirection * (MoveDistance * 0.5f);
            }
            else
            {
                // ���݈ʒu����w������Ɉړ�
                _startPosition = transform.position;
                _endPosition = transform.position + _moveDirection * MoveDistance;
            }

            _isInitialized = true;
        }

        /// <summary>�^�C�}�[�A���̈ʒu�X�V</summary>
        private void UpdatePosition()
        {
            if (!_isInitialized) return;

            // UIHub.Instance.Timer ���猻�ݎ������擾
            var timer = UIHub.Instance?.Timer;
            if (timer == null) return;

            float currentTime = timer.CurrentTime;

            // �^�C�}�[���~�܂��Ă���ꍇ�͈ʒu�X�V���Ȃ�
            if (Mathf.Approximately(currentTime, _lastUpdateTime)) return;
            _lastUpdateTime = currentTime;

            // �ړ��̎������v�Z�i�������� = ���� / ���x * 2�j
            float cycleDuration = (MoveDistance / MoveSpeed) * 2f;
            if (cycleDuration <= 0f) return;

            // ���݂̎��������Ԃ��擾�i0�`cycleDuration�j
            float cycleTime = currentTime % cycleDuration;

            // 0�`1�̐i�s�x�ɕϊ��i�������l���j
            float progress;
            if (cycleTime <= cycleDuration * 0.5f)
            {
                // �O���F�J�n���I��
                progress = (cycleTime / (cycleDuration * 0.5f));
            }
            else
            {
                // �㔼�F�I�����J�n
                progress = 1f - ((cycleTime - cycleDuration * 0.5f) / (cycleDuration * 0.5f));
            }

            // Easing�K�p
            float easedProgress = DOVirtual.EasedValue(0f, 1f, progress, EaseType);

            // �ʒu���X�V
            transform.position = Vector3.Lerp(_startPosition, _endPosition, easedProgress);
        }

        /// <summary>�G�f�B�^�ł̉���</summary>
        void OnDrawGizmosSelected()
        {
            if (!_isInitialized && Application.isPlaying) return;

            // ����������Ă��Ȃ��ꍇ�͉��v�Z
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

            // �ړ��͈͂̕`��
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, endPos);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos, 0.2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(endPos, 0.2f);

            // �������
            Gizmos.color = Color.yellow;
            Vector3 arrowStart = startPos + (endPos - startPos) * 0.3f;
            Vector3 arrowEnd = startPos + (endPos - startPos) * 0.7f;
            Gizmos.DrawLine(arrowStart, arrowEnd);
        }
    }
}