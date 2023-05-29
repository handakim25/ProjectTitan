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
            public static readonly int HasMoveInput = Animator.StringToHash("HasMoveInput");
            public static readonly int IsWalk = Animator.StringToHash("IsWalk");
            public static readonly int IsGround = Animator.StringToHash("IsGround");
            public static readonly int IsJump = Animator.StringToHash("IsJump");
            public static readonly int DashTrigger = Animator.StringToHash("DashTrigger");
            public static readonly int IsDash = Animator.StringToHash("IsDash");
            public static readonly int IsArmed = Animator.StringToHash("IsArmed");
        }
    }
}
