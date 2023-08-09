using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Attack Wait")]
    public class AttackWaitDecision : WaitDecision
    {
        public override void OnEnableDecision(StateController controller)
        {
            controller.Variables.AttackStartTime = Time.time;
        }

        public override bool Decide(StateController controller)
        {
            return Time.time - controller.Variables.AttackStartTime > controller.GeneralStats.AttackWaitDuration;
        }
    }
}
