using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>タイマー表示UI（バー+テキスト）</summary>
    public class TimerUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI TimerText { get; private set; }
        [field: SerializeField] public Image TimerBar { get; private set; }
        [field: SerializeField] public float InitialTime { get; private set; } = 60f;
        [field: SerializeField] public bool AutoStart { get; private set; } = true;

        private float currentTime;
        private bool isRunning = false;

        public event System.Action OnTimerFinished;

        public float CurrentTime => currentTime;
        public float Progress => currentTime / InitialTime;

        void Start()
        {
            currentTime = InitialTime;
            UpdateDisplay();

            if (AutoStart)
            {
                StartTimer();
            }
        }

        void Update()
        {
            if (isRunning)
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    isRunning = false;
                    OnTimerFinished?.Invoke();
                }

                UpdateDisplay();
            }
        }

        /// <summary>表示更新</summary>
        private void UpdateDisplay()
        {
            // テキスト更新
            if (TimerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                TimerText.text = $"{minutes:00}:{seconds:00}";
            }

            // バー更新
            if (TimerBar != null)
            {
                TimerBar.fillAmount = Progress;
            }
        }

        /// <summary>タイマー開始</summary>
        public void StartTimer()
        {
            isRunning = true;
        }

        /// <summary>タイマー停止</summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        /// <summary>時間を追加</summary>
        public void AddTime(float additionalTime)
        {
            currentTime = Mathf.Clamp(currentTime + additionalTime, 0f, InitialTime);
            UpdateDisplay();
        }
    }
}