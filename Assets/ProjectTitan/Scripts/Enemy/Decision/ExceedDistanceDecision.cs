using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [CreateAssetMenu(menuName = "Enemy/AI/Decisions/Exceed Distance")]
    public class ExceedDistanceDecision : Decision
    {
        public float MaxBattleDistanceOveride = 0f;

        public override bool Decide(StateController controller)
        {
            float maxBattleDistance = MaxBattleDistanceOveride > 0f ? MaxBattleDistanceOveride : controller.GeneralStats.MaxBattlDistance;
            return Vector3.Distance(controller.transform.position, controller.Variables.ReturnPos) > maxBattleDistance;
        }
    }
}
