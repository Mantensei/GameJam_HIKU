using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>�B�e�\�c��UI</summary>
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

        /// <summary>�\���X�V</summary>
        private void UpdateDisplay()
        {
            if (ShotText == null) return;

            if (InfiniteShots)
            {
                ShotText.text = "��";
            }
            else
            {
                ShotText.text = $"�~ {currentShots}";
            }
        }

        /// <summary>�B�e�����s�i�������Ɏc������j</summary>
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

        /// <summary>�c����ǉ�</summary>
        public void AddShots(int additionalShots)
        {
            if (InfiniteShots) return;

            currentShots = Mathf.Clamp(currentShots + additionalShots, 0, MaxShots);
            UpdateDisplay();
        }

        /// <summary>�c���𒼐ڐݒ�</summary>
        public void SetShots(int shots)
        {
            if (InfiniteShots) return;

            currentShots = Mathf.Clamp(shots, 0, MaxShots);
            UpdateDisplay();
        }
    }
}