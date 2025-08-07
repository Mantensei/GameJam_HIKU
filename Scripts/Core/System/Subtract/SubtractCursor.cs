using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_HIKU
{
    /// <summary>カーソルアイコン反応制御</summary>
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

        /// <summary>通常カーソルに設定</summary>
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

        /// <summary>ターゲット用カーソルに設定</summary>
        public void SetTargetCursor()
        {
            if (TargetCursor != null)
            {
                Cursor.SetCursor(TargetCursor, CursorHotspot, CursorMode.Auto);
                isTargetMode = true;
            }
        }

        /// <summary>カーソル状態の取得</summary>
        public bool IsTargetMode() => isTargetMode;
    }        

    /// <summary>パルス効果</summary>
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