using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU.FX
{
    public class SubtractArtDirector : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Parent)]
        SubtractManager subtractManager;

        [GetComponent(HierarchyRelation.Parent)]
        SubtractSkill subtractSkill;

        private void Start()
        {
            subtractSkill.OnSkillActivated += OnSkillActivated;
            subtractSkill.OnSkillDeactivated += OnSkillDeactivated;
            subtractManager.onRemoveSuccess += Flash;
        }

        void OnSkillActivated()
        {
            ShaderControllerHub.Instance.timeStopController.StartTimeStop();
        }

        void OnSkillDeactivated()
        {
            ShaderControllerHub.Instance.timeStopController.EndTimeStop();
        }

        void Flash()
        {
            ShaderControllerHub.Instance.flashEffectController.TriggerFlash();
        }
    } 
}
