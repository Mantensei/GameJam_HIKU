using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

namespace GameJam_HIKU
{
    public enum ItemType
    {
        Other,
        Life,
        Timer,
        Camera,
    }

    public class Item : MonoBehaviour
    {
        [SerializeField] ItemType itemType = ItemType.Other;
        bool _touchFlag = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_touchFlag) return;

            if (collision.gameObject.TryGetComponent<IGetTeam>(out var team))
            {
                if(team.TeamID == 1)
                {
                    switch (itemType)
                    {
                        case ItemType.Other:
                            break;
                        case ItemType.Life:
                            Life();
                            break;
                        case ItemType.Timer:
                            Timer();
                            break;
                        case ItemType.Camera:
                            Camera();
                            break;
                    }
                }

                _touchFlag = true;
                Destroy(gameObject);
            }
        }

        void Life()
        {
            UIHub.Instance?.HPBar.Heal(1000);
        }

        void Timer()
        {

        }

        void Camera()
        {
            UIHub.Instance?.ShotCounter.AddShots(1);
        }
    }
}