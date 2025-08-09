using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameJam_HIKU
{
    public class Sora_Profile : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Parent)]
        CharacterProfile characterProfile;

        float timer = 0f;
        float time = 1f;
        [SerializeField] GameObject healParticle;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                timer = 0f;
                Skill();
            }
        }

        void Skill()
        {
            foreach(var hpBar in FindObjectsByType<HPBarUI>(FindObjectsSortMode.None))
            {
                hpBar.Heal(100f);
                Instantiate(healParticle, hpBar.transform.position, Quaternion.identity);

                var balloon = new string[]
                {
                    "みんなー！\r\n今日も\r\n来てくれて\r\nありがとー！",
                    "それじゃあ\r\n一曲目！\r\n\r\nれっつごー！",
                    "みんな\r\n一緒に♪\r\n\r\nららら～♪",
                    "元気出して\r\nいこー！",
                    "みんなー！\r\n大好きー！\r\n\r\n明日も\r\n来てね！",
                };

                characterProfile.SetBalloonText(balloon.GetRandomElementOrDefault());
            }
        }
    } 
}
