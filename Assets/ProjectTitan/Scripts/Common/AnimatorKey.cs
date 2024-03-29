using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character
{
    public static class AnimatorKey
    {
        public static class Player
        {
            // Locomotion
            public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
            // Move Input 여부
            public static readonly int HasMoveInput = Animator.StringToHash("HasMoveInput");
            public static readonly int IsWalk = Animator.StringToHash("IsWalk");
            public static readonly int IsGround = Animator.StringToHash("IsGround");
            public static readonly int IsJump = Animator.StringToHash("IsJump");
            public static readonly int DashTrigger = Animator.StringToHash("DashTrigger");
            public static readonly int IsArmed = Animator.StringToHash("IsArmed");
            public static readonly int IsDash = Animator.StringToHash("IsDash");

            // Attack
            public static readonly int BasicIndex = Animator.StringToHash("BasicIndex");
            public static readonly int BasicTrigger = Animator.StringToHash("BasicTrigger");
            public static readonly int SkillIndex = Animator.StringToHash("SkillIndex");
            public static readonly int SkillTrigger = Animator.StringToHash("SkillTrigger");
            public static readonly int HasCombo = Animator.StringToHash("HasCombo");
            public static readonly int BasicStateTime = Animator.StringToHash("BasicStateTime");
        }

        public static class Enemy
        {
            public static readonly int Speed = Animator.StringToHash("Speed");
            public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
            public static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
            public static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
            public static readonly int HitTrigger = Animator.StringToHash("HitTriger");
            public static readonly int IsHitEnd = Animator.StringToHash("IsHitEnd");
        }
    }
}
