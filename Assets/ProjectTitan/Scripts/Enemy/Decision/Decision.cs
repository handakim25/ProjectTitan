using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Enemy.FSM
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
        
        public delegate bool HandleTargets(StateController controller, bool hasTargets, Collider[] targetInRadius);

        public static bool CheckTargetsInRadius(StateController controller, float radius, HandleTargets handleTargets)
        {
            throw new System.NotImplementedException();
        }
    }
}
