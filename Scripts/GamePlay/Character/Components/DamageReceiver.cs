using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class DamageReceiver : MonoBehaviour, IDamageable
    {
        [GetComponent]
        Animation2DRegisterer _anim;

        public void TakeDamage(DamageInfo damageInfo)
        {
            _anim.Play();
        }
    }
}