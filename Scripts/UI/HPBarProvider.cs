using GameJam_HIKU;
using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class HPBarProvider : MonoBehaviour, IHPProvider
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Parent)]
        IPlayer player;

        [GetComponent(HierarchyRelation.Children)]
        HPBarUI HPBarUI;

        [GetComponent(HierarchyRelation.Children)]
        Canvas HPBarCanvas;

        public int HP = 100;

        void Start()
        {
            HPBarUI.SetMaxHP(HP);
            HPBarCanvas.transform.SetParent(null);
            HPBarCanvas.transform.rotation = Quaternion.identity;

            HPBarUI.onHPZero += () =>
            {
                if (player?.IsSafe() == true)
                    Destroy(player.gameObject);
                else
                    Destroy(gameObject);
            };

            HPBarUI.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            HPBarCanvas.transform.position = transform.position;
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            HPBarUI.gameObject.SetActive(true);
            HPBarUI?.TakeDamage(damageInfo.Damage);
        }

        void OnDestroy()
        {
            if(this != null && HPBarCanvas != null)
                Destroy(HPBarCanvas.gameObject);
        }
    } 
}
