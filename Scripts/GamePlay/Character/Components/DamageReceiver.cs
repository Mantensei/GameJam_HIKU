using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class DamageReceiver : MonoBehaviour, IDamageable
    {
        float invinsibleTimer;
        [field:SerializeField] public float invinsibleTime { get; private set; } = 1;

        [GetComponent]
        ActionStateController stateController;

        [GetComponent]
        Animation2DRegisterer animRegisterer;

        void Start()
        {
            animRegisterer?.OnCompleteAction(_ => stateController.UnlockState());
        }

        void Update()
        {
            invinsibleTimer -= Time.deltaTime;
        }


        public void TakeDamage(DamageInfo damageInfo)
        {
            if (invinsibleTimer > 0)
            {
                damageInfo.Result = DamageResult.Missed;
                return;
            }

            if(!damageInfo.HasTag(DamageType.No_Invinsible))
            {
                invinsibleTimer = invinsibleTime;
                stateController?.TryForceLock(() => animRegisterer?.Play());
            }

            foreach(var provider in GetComponentsInChildren<IHPProvider>())
            {
                provider.TakeDamage(damageInfo);
            }
        }
    }
}