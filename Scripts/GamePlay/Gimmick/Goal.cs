using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam_HIKU
{
    public class Goal : MonoBehaviour
    {
        [SerializeField]int next = -1;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                if(collision.GetComponent<IGetTeam>()?.TeamID == 1)
                {
                    LoadStage();
                    Destroy(this);
                }
            }
        }

        void LoadStage()
        {
            string footer = "";

            if(next < 0)
            {
                var current = SceneManager.GetActiveScene();
                if(int.TryParse(current.name.Split("_").LastOrDefault(), out var num))
                {
                    footer = (++num).ToString("D2");
                }
                else
                {
                    return;
                }
            }
            else
            {
                footer = next.ToString("D2");
            }

            var load = SceneManager.LoadSceneAsync("Scene_" + footer, LoadSceneMode.Single);
            load.completed += (x) =>
            {
                
            };
        }
    }
}