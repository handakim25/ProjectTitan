using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Enemy.FSM
{
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
    }
}
