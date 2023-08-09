using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Wait")]
    public class WaitDecision : Decision
    {
        public float WaitTime = 2f;

        public override void OnEnableDecision(StateController controller)
        {
            controller.Variables.WaitStartTime = Time.time;
        }

        public override bool Decide(StateController controller)
        {
            return Time.time - controller.Variables.WaitStartTime > WaitTime;
        }
    }
}
