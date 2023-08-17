using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    // State : one state -> multiple actions, multiple Transitions(decision + target state)
    // Transition : Decision -> result state
    // 1st. DoActions
    // 2nd. CheckTransitions
    [CreateAssetMenu(menuName = "Enemy/AI/State")]
    public class State : ScriptableObject
    {
        [Tooltip("Update from lower index")]
        public Action[] actions;
        [Tooltip("Update from lower index")]
        public Transition[] transitions;
        public bool IsCombat;

#if UNITY_EDITOR
        [TextArea(2, 5)]
        [SerializeField] private string Description;
#endif

        public void DoActions(StateController controller)
        {
            foreach(Action action in actions)
            {
                action.Act(controller);
            }
        }

        /// <summary>
        /// Called when state enable
        /// </summary>
        /// <param name="controller"></param>
        public void OnEnableActions(StateController controller)
        {
            foreach(Action action in actions)
            {
                action.OnReadyAction(controller);
            }
            foreach(Transition transition in transitions)
            {
                transition.Decision.OnEnableDecision(controller);
            }
        }

        public void OnDisableActions(StateController controller)
        {
            foreach(Action action in actions)
            {
                action.OnDisableAction(controller);
            }
            foreach(Transition transition in transitions)
            {
                transition.Decision.OnDisableDecision(controller);
            }
        }

        public void CheckTransitions(StateController controller)
        {
            State prevState = controller.currentState;
            foreach(Transition transition in transitions)
            {
                bool decision = transition.Decision.Decide(controller);
                if(decision)
                {
                    controller.TransitionToState(transition.TrueState, transition.Decision);
                }
                else
                {
                    controller.TransitionToState(transition.FalseState, transition.Decision);
                }

                if(controller.currentState != this)
                {
#if UNITY_EDITOR
                    if(controller.DebugMode)
                    {
                        Debug.Log($"Change State : {this.name} -> {controller.currentState.name} / Decision : {transition.Decision.name}");
                    }
#endif
                    prevState.OnDisableActions(controller);
                    controller.currentState.OnEnableActions(controller);
                    break;
                }
            }
        }
    }
}
