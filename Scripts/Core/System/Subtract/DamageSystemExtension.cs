using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>
    /// DamageInfo��DamageType�֘A�g�����\�b�h
    /// </summary>
    public static class DamageInfoExtensions
    {
        /// <summary>
        /// DamageType���^�O�Ƃ��Ēǉ�
        /// </summary>
        public static DamageInfo AddTag(this DamageInfo damageInfo, DamageType damageType)
        {
            return damageInfo.AddTag(damageType.ToString());
        }

        /// <summary>
        /// DamageType�̃^�O���܂܂�Ă��邩�`�F�b�N
        /// </summary>
        public static bool HasTag(this DamageInfo damageInfo, DamageType damageType)
        {
            return damageInfo.HasTag(damageType.ToString());
        }

        /// <summary>
        /// �V�X�e���폜�p��DamageInfo���쐬
        /// </summary>
        public static DamageInfo CreateSystemDamage(GameObject attacker = null)
        {
            return new DamageInfo(0, attacker).AddTag(DamageType.System);
        }
    }

        /// <summary>
    /// �_���[�W�̎�ނ��`
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// �ʏ�̕����_���[�W�i�����I�w��p�A�����݊����̂��ߎc���j
        /// </summary>
        Physical = 0,
        
        /// <summary>
        /// �V�X�e���I�폜�i�u�����v����ɂ�鏜���j
        /// </summary>
        System = 1
    }
}