using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace GameJam_HIKU
{
    /// <summary>
    /// クリックによる削除が可能なオブジェクト用コンポーネント
    /// IDamageableを実装し、システム削除とダメージを区別して処理
    /// </summary>
    public class RemovableObject : MonoBehaviour, IDamageable
    {
        [field : SerializeField] public float removeDelay { get; set; } = 0f;
        [field : SerializeField] public DisableType disableType { get; set; }

        private bool isRemoving = false;

        // イベント
        public event Action<DamageInfo> onClicked;

        /// <summary>
        /// IDamageableの実装 - ダメージまたは削除処理を判定
        /// </summary>
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (isRemoving) return;

            // システム削除の場合
            if (damageInfo.HasTag(DamageType.System))
            {
                HandleSystemRemove(damageInfo);
            }
            // 通常ダメージの場合
            else
            {
                HandleDamage(damageInfo);
            }
        }

        /// <summary>
        /// システム削除処理（クリックによる削除）
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
                    Debug.Log($"{gameObject.name} は削除できません");
                    return;
                default:
                    break;
            }
        }

        void HandleDamage(DamageInfo damageInfo)
        {
            //!Todo: 子にダメージ処理を委託
            foreach(var damageable in GetComponentsInChildren<IDamageable>())
            {
                if((object)damageable == this) continue; // 自分自身は除外
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