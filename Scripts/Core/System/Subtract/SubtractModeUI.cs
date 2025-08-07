using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>スキル発動中のUI表示</summary>
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

        /// <summary>モードUI表示切り替え</summary>
        public void SetActive(bool active)
        {
            isActive = active;
            if (ModePanel != null)
            {
                ModePanel.SetActive(active);
            }
        }

        /// <summary>残り時間バー更新</summary>
        public void UpdateTimer(float remainingTime, float totalTime)
        {
            if (!isActive || TimerBar == null) return;

            float progress = totalTime > 0 ? remainingTime / totalTime : 0f;
            TimerBar.fillAmount = progress;

            // 残り時間が少ない時は色を変更
            TimerBar.color = remainingTime <= WarningThreshold ? WarningColor : ActiveColor;
        }
    } 
}