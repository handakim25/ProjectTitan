using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    // Action에 focus / straff 등을 설정할 수 있게 할 수 있을까?
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);

        /// <summary>
        /// Called when state enable
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnReadyAction(StateController controller)
        {
            
        }

        /// <summary>
        /// Called when state disable
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnDisableAction(StateController controller)
        {
            
        }

        #region Helper methods
        
        protected bool HasArrived(StateController controller)
        {
            return controller.Nav.remainingDistance <= controller.Nav.stoppingDistance && !controller.Nav.pathPending;
        }
        
        #endregion Helper methods
    }
}
