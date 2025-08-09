using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class Attacker_Kick : MonoBehaviour
    {
        [GetComponent]
        ActionStateController StateController;

        [GetComponent]
        Animation2DRegisterer Registerer;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        DamageObjectSpawner DamageObjectSpawner;

        [GetComponent]
        EventObjectTrigger _eventObjectTrigger;

        private void Start()
        {
            Registerer?.OnCompleteAction(_ => StateController?.UnlockState());
        }

        public void Attack()
        {
            StateController?.TryLock(Kick);
        }

        void Kick()
        {
            Registerer?.Play();
            DelayedActionManager.Execute(0.25f, () =>
            {
                var obj = DamageObjectSpawner.SpawnDamageObject();
                obj.onDamageApplied += (_, result) =>
                {
                    if (!(result?.Miss == true))
                    {
                        _eventObjectTrigger.SpawnPosition = obj.transform;
                        _eventObjectTrigger?.ForceExecute();
                    }
                };
            });
        }
    }
}
