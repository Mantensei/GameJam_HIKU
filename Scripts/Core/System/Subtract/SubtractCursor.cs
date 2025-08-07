using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_HIKU
{
    /// <summary>�J�[�\���A�C�R����������</summary>
    public class SubtractCursor : MonoBehaviour
    {
        [field: SerializeField] public Texture2D NormalCursor { get; private set; }
        [field: SerializeField] public Texture2D TargetCursor { get; private set; }
        [field: SerializeField] public Vector2 CursorHotspot { get; private set; } = Vector2.zero;

        private bool isTargetMode = false;

        void Start()
        {
            SetNormalCursor();
        }

        /// <summary>�ʏ�J�[�\���ɐݒ�</summary>
        public void SetNormalCursor()
        {
            if (NormalCursor != null)
            {
                Cursor.SetCursor(NormalCursor, CursorHotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            isTargetMode = false;
        }

        /// <summary>�^�[�Q�b�g�p�J�[�\���ɐݒ�</summary>
        public void SetTargetCursor()
        {
            if (TargetCursor != null)
            {
                Cursor.SetCursor(TargetCursor, CursorHotspot, CursorMode.Auto);
                isTargetMode = true;
            }
        }

        /// <summary>�J�[�\����Ԃ̎擾</summary>
        public bool IsTargetMode() => isTargetMode;
    }        

    /// <summary>�p���X����</summary>
    public class PulseEffect : MonoBehaviour
    {
        private float pulseSpeed = 2f;
        private Vector3 originalScale;

        public void Initialize(float speed)
        {
            pulseSpeed = speed;
            originalScale = transform.localScale;
        }

        void Update()
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.1f;
            transform.localScale = originalScale * pulse;
        }
    }
}