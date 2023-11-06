using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.QuestSystem
{
    /// <summary>
    /// 퀘스트 진행 사항을 저장하는 플레이어 데이터
    /// </summary>
    public class QuestProgressData
    {
        public string QuestID;
        public QuestStatus Status;
        /// <summary>
        /// Quest의 목표 달성률. 몬스터 킬, 채집 등을 기록한다.
        /// </summary>
        public int CurrentProgress;
    }
}
