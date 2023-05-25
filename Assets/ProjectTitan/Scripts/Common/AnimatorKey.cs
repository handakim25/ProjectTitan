using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character
{
    public static class AnimatorKey
    {
        public static class Player
        {
            public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
            public static readonly int IsMoving = Animator.StringToHash("IsMoving");
            public static readonly int IsWalk = Animator.StringToHash("IsWalk");
        }
    }
}
