using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Enemy.FSM
{
    [System.Serializable]
    public class Transition
    {
        public Decision Decision;
        public State TrueState;
        public State FalseState;
    }
}
