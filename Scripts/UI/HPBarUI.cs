using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>HP表示UI（バー+テキスト）</summary>
    public class HPBarUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI HPText { get; private set; }
        [field: SerializeField] public Image HPBar { get; private set; }
        [field: SerializeField] public float MaxHP { get; private set; } = 100f;

        public float currentHP { get; private set; }

        public event System.Action OnHPZero;

        public float CurrentHP => currentHP;
        public float Progress => MaxHP > 0f ? currentHP / MaxHP : 0f;

        void Start()
        {
            currentHP = MaxHP;
            UpdateDisplay();
        }

        /// <summary>表示更新</summary>
        private void UpdateDisplay()
        {
            // テキスト更新
            if (HPText != null)
            {
                HPText.text = $"{Mathf.FloorToInt(currentHP)}/{Mathf.FloorToInt(MaxHP)}";
            }

            // バー更新
            if (HPBar != null)
            {
                HPBar.fillAmount = Progress;
            }
        }

        /// <summary>ダメージを受ける</summary>
        public void TakeDamage(float damage)
        {
            currentHP = Mathf.Clamp(currentHP - damage, 0f, MaxHP);
            UpdateDisplay();

            if (currentHP <= 0f)
            {
                OnHPZero?.Invoke();
            }
        }

        /// <summary>回復</summary>
        public void Heal(float healAmount)
        {
            currentHP = Mathf.Clamp(currentHP + healAmount, 0f, MaxHP);
            UpdateDisplay();
        }

        /// <summary>HPを直接設定</summary>
        public void SetHP(float hp)
        {
            currentHP = Mathf.Clamp(hp, 0f, MaxHP);
            UpdateDisplay();
        }
    }
}