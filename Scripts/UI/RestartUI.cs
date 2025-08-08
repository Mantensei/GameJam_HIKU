using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameJam_HIKU
{
    public class RestartUI : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        Button button;

        private void Start()
        {
            button?.onClick.AddListener(RestartGame);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }

        public void RestartGame()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
