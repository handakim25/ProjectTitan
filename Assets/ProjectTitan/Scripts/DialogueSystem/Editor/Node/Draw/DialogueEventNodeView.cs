using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueEventNodeView : DialogueConditionNodeView
    {
        /// <summary>
        /// Game Event SO GUID
        /// </summary>
        public string GameEventSOGUID;
        /// <summary>
        /// Game Event의 ID
        /// </summary>
        public string TriggerName;
        /// <summary>
        /// Game Event를 어떻게 상태 변화시킬지
        /// </summary>
        public bool TriggerStatus;

        protected override void BuildView()
        {
            base.BuildView();


        }

        public override Requirement GetRequirement()
        {
            return new Requirement()
            {
                Type = Requirement.RequirementType.Trigger,
                ExpectedBool = true,
                TargetID = TriggerName,
            };
        }
    }
}
