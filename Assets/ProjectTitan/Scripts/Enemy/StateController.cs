using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Titan.Enemy.FSM;

namespace Titan.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateController : MonoBehaviour
    {
        // Read Stat Data
        // Set Stat Data

        public State currentState;
        public State remainState;

        private bool aiActive;

        [HideInInspector] public NavMeshAgent Nav;

        private void Awake()
        {
            aiActive = true;

            Nav = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if(!aiActive)
            {
                return;
            }

            currentState.DoActions(this);
            currentState.CheckTransitions(this);
        }

        public void TransitionToState(State nextState, Decision decision)
        {
            if(nextState != remainState)
            {
                currentState = nextState;
            }
        }
    }
}
