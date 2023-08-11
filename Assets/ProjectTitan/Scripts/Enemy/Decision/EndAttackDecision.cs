using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    // 임시로 구현한 디시전
    // 지금은 temp decision과 동일하다
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/End Attack")]
    public class EndAttackDecision : Decision
    {
        public float WaitTime = 2f;
        public override void OnEnableDecision(StateController controller)
        {
            controller.Variables.WaitStartTime = Time.time;
        } 

        public override void OnDisableDecision(StateController controller)
        {
            controller.Variables.AttackEndTime = Time.time;
        }

        public override bool Decide(StateController controller)
        {
            return Time.time - controller.Variables.WaitStartTime > WaitTime;
        }
    }
}
