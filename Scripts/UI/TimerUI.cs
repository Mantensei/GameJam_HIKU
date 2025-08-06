using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>�^�C�}�[�\��UI�i�o�[+�e�L�X�g�j</summary>
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

        /// <summary>�\���X�V</summary>
        private void UpdateDisplay()
        {
            // �e�L�X�g�X�V
            if (TimerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                TimerText.text = $"{minutes:00}:{seconds:00}";
            }

            // �o�[�X�V
            if (TimerBar != null)
            {
                TimerBar.fillAmount = Progress;
            }
        }

        /// <summary>�^�C�}�[�J�n</summary>
        public void StartTimer()
        {
            isRunning = true;
        }

        /// <summary>�^�C�}�[��~</summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        /// <summary>���Ԃ�ǉ�</summary>
        public void AddTime(float additionalTime)
        {
            currentTime = Mathf.Clamp(currentTime + additionalTime, 0f, InitialTime);
            UpdateDisplay();
        }
    }
}