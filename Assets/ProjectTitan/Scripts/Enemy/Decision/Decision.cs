using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController controller);

        /// <summary>
        /// Called when state enable
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnEnableDecision(StateController controller)
        {

        }

        public virtual void OnDisableDecision(StateController controller)
        {
            
        }
        
        public delegate bool HandleTargets(StateController controller, bool hasTargets, Collider[] targetInRadius);

        // @To-Do
        // NonAlloc으로 변경할 것
        // 일시적으로 핑이 튀는 현상이 있었다.
        public static bool CheckTargetsInRadius(StateController controller, float radius, HandleTargets handleTargets)
        {
            // player dead 시에는 체크할 필요가 없다.
            Collider[] targetsInRadius = Physics.OverlapSphere(controller.transform.position, radius, controller.GeneralStats.targetMask);
            return handleTargets(controller, targetsInRadius.Length > 0, targetsInRadius);
        }

#if UNITY_EDITOR
        [TextArea(2, 5)]
        [SerializeField] private string Description;
#endif
    }
}
