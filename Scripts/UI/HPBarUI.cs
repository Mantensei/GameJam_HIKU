using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameJam_HIKU
{
    /// <summary>HP�\��UI�i�o�[+�e�L�X�g�j</summary>
    public class HPBarUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI HPText { get; private set; }
        [field: SerializeField] public Image HPBar { get; private set; }
        [field: SerializeField] public float MaxHP { get; private set; } = 100f;

        public float currentHP { get; private set; }

        public event System.Action onHPZero;

        public float CurrentHP => currentHP;
        public float Progress => MaxHP > 0f ? currentHP / MaxHP : 0f;

        private void Awake()
        {
            currentHP = MaxHP;
        }

        void Start()
        {
            UpdateDisplay();
        }

        /// <summary>�\���X�V</summary>
        private void UpdateDisplay()
        {
            // �e�L�X�g�X�V
            if (HPText != null)
            {
                HPText.text = $"{Mathf.FloorToInt(currentHP)}/{Mathf.FloorToInt(MaxHP)}";
            }

            // �o�[�X�V
            if (HPBar != null)
            {
                HPBar.fillAmount = Progress;
            }
        }

        /// <summary>�_���[�W���󂯂�</summary>
        public void TakeDamage(float damage)
        {
            currentHP = Mathf.Clamp(currentHP - damage, 0f, MaxHP);
            UpdateDisplay();

            if (currentHP <= 0f)
            {
                onHPZero?.Invoke();
            }
        }

        /// <summary>��</summary>
        public void Heal(float healAmount)
        {
            currentHP = Mathf.Clamp(currentHP + healAmount, 0f, MaxHP);
            UpdateDisplay();
        }

        public void SetMaxHP(float hp)
        {
            currentHP = hp;
            MaxHP = hp;
            UpdateDisplay();
        }

        /// <summary>HP�𒼐ڐݒ�</summary>
        public void SetHP(float hp)
        {
            currentHP = Mathf.Clamp(hp, 0f, MaxHP);
            UpdateDisplay();
        }
    }
}