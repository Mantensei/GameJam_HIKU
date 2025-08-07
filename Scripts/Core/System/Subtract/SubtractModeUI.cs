using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>�X�L����������UI�\��</summary>
namespace GameJam_HIKU
{
    public class SubtractModeUI : MonoBehaviour
    {
        [field: SerializeField] public GameObject ModePanel { get; private set; }
        [field: SerializeField] public Image TimerBar { get; private set; }
        [field: SerializeField] public Color ActiveColor { get; private set; } = Color.cyan;
        [field: SerializeField] public Color WarningColor { get; private set; } = Color.red;
        [field: SerializeField] public float WarningThreshold { get; private set; } = 1f;

        private bool isActive = false;

        void Start()
        {
            SetActive(false);
        }

        /// <summary>���[�hUI�\���؂�ւ�</summary>
        public void SetActive(bool active)
        {
            isActive = active;
            if (ModePanel != null)
            {
                ModePanel.SetActive(active);
            }
        }

        /// <summary>�c�莞�ԃo�[�X�V</summary>
        public void UpdateTimer(float remainingTime, float totalTime)
        {
            if (!isActive || TimerBar == null) return;

            float progress = totalTime > 0 ? remainingTime / totalTime : 0f;
            TimerBar.fillAmount = progress;

            // �c�莞�Ԃ����Ȃ����͐F��ύX
            TimerBar.color = remainingTime <= WarningThreshold ? WarningColor : ActiveColor;
        }
    } 
}