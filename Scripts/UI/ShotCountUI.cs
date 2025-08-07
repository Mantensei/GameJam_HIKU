using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>撮影可能残数UI</summary>
    public class ShotCountUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI ShotText { get; private set; }
        [field: SerializeField] public int MaxShots { get; private set; } = 10;
        [field: SerializeField] public bool InfiniteShots { get; private set; } = false;

        private int currentShots;

        public event System.Action OnShotsEmpty;

        public int CurrentShots => currentShots;
        public bool CanShoot => InfiniteShots || currentShots > 0;

        void Start()
        {
            currentShots = MaxShots;
            UpdateDisplay();
        }

        /// <summary>表示更新</summary>
        private void UpdateDisplay()
        {
            if (ShotText == null) return;

            if (InfiniteShots)
            {
                ShotText.text = "∞";
            }
            else
            {
                ShotText.text = $"× {currentShots}";
            }
        }

        /// <summary>撮影を試行（成功時に残数消費）</summary>
        public bool TryShoot()
        {
            if (!CanShoot) return false;

            if (!InfiniteShots)
            {
                currentShots--;
                UpdateDisplay();

                if (currentShots <= 0)
                {
                    OnShotsEmpty?.Invoke();
                }
            }

            return true;
        }

        /// <summary>残数を追加</summary>
        public void AddShots(int additionalShots)
        {
            if (InfiniteShots) return;

            currentShots = Mathf.Clamp(currentShots + additionalShots, 0, MaxShots);
            UpdateDisplay();
        }

        /// <summary>残数を直接設定</summary>
        public void SetShots(int shots)
        {
            if (InfiniteShots) return;

            currentShots = Mathf.Clamp(shots, 0, MaxShots);
            UpdateDisplay();
        }
    }
}