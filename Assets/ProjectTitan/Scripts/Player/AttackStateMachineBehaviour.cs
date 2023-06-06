using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Battle;

/// Note
/// 1. State Enter -> State Exit -> New State Enter -> New State Eixt 순으로 호출된다.  
/// 2. State Machine Enter가 가장 먼저 호출된다
/// 3. State Exit으로 나가고나서도 StateMove로 이동해야 종료된다
/// 4. State Update는 State Exit 이후에 호출되지 않는다.
/// 5. State Machine Exit이 StateExit 보다 먼저 호출된다.
/// 아마 StateMachienExit에서 다음 트랜지션이 결정되고
/// 트랜지션이 완료가 된다면 State Exit이 되는 것으로 추정한다
/// Animation 정보
/// 1. AnimatorClipInfo
/// 2. AnimatorStateInfo
/// Current / Next로 2가지 버전이 있다
namespace Titan.Character.Player
{
    public class AttackStateMachineBehaviour : StateMachineBehaviour
    {
        public event System.Action OnAttackStart;
        public event System.Action OnAttackEnd;

        public AttackType AttackType = AttackType.Basic;
#if UNITY_EDITOR
        public bool debugMode = true;
        private Dictionary<int, string> DebugDictionary = new Dictionary<int, string>();
#endif
        private void Awake()
        {
#if UNITY_EDITOR
            DebugDictionary[Animator.StringToHash("None")] = "None";
            DebugDictionary[Animator.StringToHash("Skill0")] = "Skill0";
            DebugDictionary[Animator.StringToHash("Selector")] = "Selector";
#endif
        }

#if UNITY_EDITOR
        public string ChangeToStr(int hash)
        {
            return DebugDictionary.TryGetValue(hash, out var value) ? value : hash.ToString();
        }
#endif

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnAttackStart?.Invoke();

#if UNITY_EDITOR
            if (debugMode)
                Debug.Log($"--StateEnter / State Info : {ChangeToStr(stateInfo.shortNameHash)}");
#endif
        }

        // Called at each Update frame except for the first and last frame.
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log($"StateUpdate / State info : {ChangeToStr(stateInfo.shortNameHash)}");
        }

        /// Note
        /// State Enter / Exit이 짝을 이루어서 현재의 State가 Update 되는 동안 호출된다.
        /// 아마 Transition 중에도 호출되는 것으로 추정한다.
        /// 인자로 오는 StateInfo에서는 자기자신이 호출되지만
        /// GetCurrentAnimatorStateInfo의 경우는 다음 것이 호출된다.(아마 blending 중?)
        /// GetNextAnimatorStateInfo의 경우는 0이 호출된다. 아마 바로 다음과의 중간 과정이라서 0인 것 같다

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
#if UNITY_EDITOR
            if (debugMode)
            {
                Debug.Log($"--StateExit / state info : {ChangeToStr(stateInfo.shortNameHash)}");
                // NextAnimatorInfo : 0
                // var animatorStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                // Debug.Log($"{animatorStateInfo.shortNameHash}");
                var currentAnimatorStateInfoDebug = animator.GetCurrentAnimatorStateInfo(layerIndex);
                Debug.Log($"Exit : {ChangeToStr(currentAnimatorStateInfoDebug.shortNameHash)}");
            }
#endif

            // 일단 임시로 코드를 작성한다. 
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentAnimatorStateInfo.shortNameHash == Animator.StringToHash("None"))
                OnAttackEnd?.Invoke();
        }

        // Called right after MonoBehaviour.OnAnimatorMove.
        // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     Debug.Log($"StateMove / state info : {ChangeToStr(stateInfo.shortNameHash)} / frame count : {Time.frameCount}");
        // }

        // Called right after MonoBehaviour.OnAnimatorIK.
        // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     Debug.Log($"OnStateIK");
        // }

        /// 가장 먼저 호출된다
        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
#if UNITY_EDITOR
            if (debugMode)
                Debug.Log($"----StateMachineEnter----");
#endif
        }

        /// 가장 나중은 아니다. 뒤에 OnStateExit이 호출된다
        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
#if UNITY_EDITOR
            if (debugMode)
                Debug.Log($"----StateMachineExit----");
#endif
        }
    }
}
