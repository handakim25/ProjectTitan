using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    /// <summary>
    /// Run-time 선택지 데이터
    /// </summary>
    [System.Serializable]
    public class Choice
    {
        /// <summary>
        /// 선택지 텍스트
        /// </summary>
        public string ChoiceText;
        /// <summary>
        /// 선택지를 선택했을 때 다음 노드 ID
        /// </summary>
        public string NextNode;
        /// <summary>
        /// 선택지가 활성화되는 조건
        /// </summary>
        public Condition Condition;
        public ChoiceType ChoiceType;
    }

    // @To-DO
    // 다음에는 선택지를 조사해서 해결할 수 있도록 할 것, Graph 이론 쪽을 참고할 것
    public enum ChoiceType
    {
        /// <summary>
        /// 대화 종료 선택지
        /// </summary>
        EndChoice,
        /// <summary>
        /// 대화 계속 선택지, 딱히 분기점이 존재하지 않는 선택지
        /// </summary>
        ContinueChoice,
        /// <summary>
        /// 대화를 하고 돌아오는 선택지
        /// </summary>
        LoopBackChoice,
    }
}
