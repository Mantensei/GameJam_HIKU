using MantenseiLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    public class UIHub : SingletonMonoBehaviour<UIHub>
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public HPBarUI HPBar { get; private set; }

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public TimerUI Timer { get; private set; }

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public ShotCountUI ShotCounter { get; private set; }
    } 
}
