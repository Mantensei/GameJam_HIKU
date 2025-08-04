using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// DamageInfoのDamageType関連拡張メソッド
    /// </summary>
    public static class DamageInfoExtensions
    {
        /// <summary>
        /// DamageTypeをタグとして追加
        /// </summary>
        public static DamageInfo AddTag(this DamageInfo damageInfo, DamageType damageType)
        {
            return damageInfo.AddTag(damageType.ToString());
        }

        /// <summary>
        /// DamageTypeのタグが含まれているかチェック
        /// </summary>
        public static bool HasTag(this DamageInfo damageInfo, DamageType damageType)
        {
            return damageInfo.HasTag(damageType.ToString());
        }

        /// <summary>
        /// システム削除用のDamageInfoを作成
        /// </summary>
        public static DamageInfo CreateSystemDamage(GameObject attacker = null)
        {
            return new DamageInfo(0, attacker).AddTag(DamageType.System);
        }
    }

        /// <summary>
    /// ダメージの種類を定義
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// 通常の物理ダメージ（明示的指定用、既存互換性のため残す）
        /// </summary>
        Physical = 0,
        
        /// <summary>
        /// システム的削除（「引く」操作による除去）
        /// </summary>
        System = 1
    }
}