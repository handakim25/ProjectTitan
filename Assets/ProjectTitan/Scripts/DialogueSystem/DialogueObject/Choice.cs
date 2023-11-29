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
        /// 선택지를 선택했을 때 다음 노드
        /// </summary>
        public string NextNode;
        /// <summary>
        /// 선택지가 활성화되는 조건
        /// </summary>
        public Condition Condition;
    }
}
