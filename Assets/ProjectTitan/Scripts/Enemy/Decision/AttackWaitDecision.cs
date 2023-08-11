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
            // 여러 상태에서 공유하기 위해서는 설정하는 부분을 공통으로 묶어야 한다.
            // controller.Variables.AttackStartTime = Time.time;
        }

        public override bool Decide(StateController controller)
        {
            return Time.time - controller.Variables.AttackEndTime > controller.GeneralStats.AttackWaitDuration;
        }
    }
}
