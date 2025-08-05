using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace GameJam_HIKU
{
    /// <summary>
    /// �N���b�N�ɂ��폜���\�ȃI�u�W�F�N�g�p�R���|�[�l���g
    /// IDamageable���������A�V�X�e���폜�ƃ_���[�W����ʂ��ď���
    /// </summary>
    public class RemovableObject : MonoBehaviour, IDamageable
    {
        [field : SerializeField] public float removeDelay { get; set; } = 0f;
        [field : SerializeField] public DisableType disableType { get; set; }

        private bool isRemoving = false;

        // �C�x���g
        public event Action<DamageInfo> onClicked;

        /// <summary>
        /// IDamageable�̎��� - �_���[�W�܂��͍폜�����𔻒�
        /// </summary>
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (isRemoving) return;

            // �V�X�e���폜�̏ꍇ
            if (damageInfo.HasTag(DamageType.System))
            {
                HandleSystemRemove(damageInfo);
            }
            // �ʏ�_���[�W�̏ꍇ
            else
            {
                HandleDamage(damageInfo);
            }
        }

        /// <summary>
        /// �V�X�e���폜�����i�N���b�N�ɂ��폜�j
        /// </summary>
        private void HandleSystemRemove(DamageInfo damageInfo)
        {
            onClicked?.Invoke(damageInfo);

            switch (disableType)
            {
                case DisableType.Destroy:
                    isRemoving = true;
                    DelayedActionManager.Execute(removeDelay, ExecuteDestroy);
                    break;
                case DisableType.Deactivate:
                    isRemoving = true;
                    DelayedActionManager.Execute(removeDelay, ExecuteDisable);
                    break;
                case DisableType.Damage:
                    DelayedActionManager.Execute(removeDelay, () => HandleDamage(damageInfo));
                    break;
                case DisableType.None:
                    Debug.Log($"{gameObject.name} �͍폜�ł��܂���");
                    return;
                default:
                    break;
            }
        }

        void HandleDamage(DamageInfo damageInfo)
        {
            //!Todo: �q�Ƀ_���[�W�������ϑ�
            foreach(var damageable in GetComponentsInChildren<IDamageable>())
            {
                if((object)damageable == this) continue; // �������g�͏��O
                damageable.TakeDamage(damageInfo);
            }
        }

        void ExecuteDestroy()
        {
            Destroy(gameObject);
        }

        void ExecuteDisable()
        {
            gameObject.SetActive(false);
        }
    }

    public enum DisableType
    {
        Destroy,
        Deactivate,
        Damage,
        None,
    }
}