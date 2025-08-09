using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameJam_HIKU
{
    public class CharacterProfile : MonoBehaviour
    {
        [field:SerializeField] public TextMeshProUGUI Name_Text { get; private set; }
        [field:SerializeField] public Image Icon_Img { get; private set; }

        [field:SerializeField] public TextMeshProUGUI Balloon_Text { get; private set; }

        public CharacterProfileData profileData;

        public void SetBalloonText(string description)
        {
            Balloon_Text.text = description;
        }

        public void SetProfileData(CharacterProfileData profileData)
        {
            Name_Text.text = profileData.name;
            Balloon_Text.text = profileData.normalDescription;
        }
    } 

    public class CharacterProfileData
    {
        public string name;
        public string normalDescription;
        public string skillDescription;
        public string removeDescription;
    }
}
