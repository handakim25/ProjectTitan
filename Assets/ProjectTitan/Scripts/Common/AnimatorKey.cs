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
            // Move Input 여부
            public static readonly int IsMoving = Animator.StringToHash("IsMoving");
            public static readonly int IsWalk = Animator.StringToHash("IsWalk");
            public static readonly int IsGround = Animator.StringToHash("IsGround");
        }
    }
}
