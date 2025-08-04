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
        [field : SerializeField] public float removeDelay { get; private set; } = 0f;
        [field : SerializeField] public DisableType disableType { get; private set; }

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
                    DelayedActionManager.Execute(removeDelay, ExecuteDestroy);
                    break;
                case DisableType.Deactivate:
                    DelayedActionManager.Execute(removeDelay, ExecuteDisable);
                    break;
                case DisableType.None:
                    Debug.Log($"{gameObject.name} は削除できません");
                    return;
                default:
                    break;
            }           
        }

        /// <summary>
        /// 通常ダメージ処理
        /// </summary>
        private void HandleDamage(DamageInfo damageInfo)
        {
            Debug.Log($"{gameObject.name} がダメージを受けました: {damageInfo.Damage}");

            //!Todo: 子にダメージ処理を委託
        }

        /// <summary>
        /// 実際の削除実行
        /// </summary>
        private void ExecuteDestroy()
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
        None,
    }
}