using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_HIKU.FX
{
    public class ShaderControllerHub : SingletonMonoBehaviour<ShaderControllerHub>
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public TimeStopEffectController timeStopController { get; private set; }

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public FlashEffectController flashEffectController { get; private set; }
    }

}