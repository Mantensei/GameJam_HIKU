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
                    "�݂�ȁ[�I\r\n������\r\n���Ă����\r\n���肪�Ɓ[�I",
                    "���ꂶ�Ⴀ\r\n��ȖځI\r\n\r\n������[�I",
                    "�݂��\r\n�ꏏ�Ɂ�\r\n\r\n����`��",
                    "���C�o����\r\n�����[�I",
                    "�݂�ȁ[�I\r\n��D���[�I\r\n\r\n������\r\n���ĂˁI",
                };

                characterProfile.SetBalloonText(balloon.GetRandomElementOrDefault());
            }
        }
    } 
}
