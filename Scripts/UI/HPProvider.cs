using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class HPProvider : MonoBehaviour, IHPProvider
    {
        public void TakeDamage(DamageInfo damageInfo)
        {
            var damage = damageInfo.Damage;

            UIHub.Instance.HPBar.TakeDamage(damage);
        }
    }

    public interface IHPProvider : IMonoBehaviour
    {
        public void TakeDamage(DamageInfo damageInfo);
    }
}