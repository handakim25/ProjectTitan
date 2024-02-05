using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public interface IEvent
    {

    }

    public struct EnemyDeadEvent : IEvent
    {
        public string EnemyID;
        public bool IsBoss; // 현재는 이렇게 구현할 것, 추후에는 EnemyData를 통해서 구현할 것
    }

    public struct ItemCollectedEvent : IEvent
    {
        public int ItemID;
        public int Count;
    }

    public struct ItemUsedEvent : IEvent
    {
        public int ItemID;
        public int Count;
    }

    public struct GameEventTriggeredEvent : IEvent
    {
        public string EventName;
        public bool TriggerStatus;
    }

    /// <summary>
    /// <para> QuestStatus : Received : 퀘스트를 수락한다.</para>
    /// <para> QuestStatus : Completed : 퀘스트를 완료한다.</para>
    /// <para> QuestStatus : Failed : 퀘스트를 실패한다.</para>
    /// </summary>
    public struct QuestEvent : IEvent
    {
        public string QuestID;
        public QuestSystem.QuestStatus Status;
    }

    public enum EventList
    {
        EnemyDeadEvent,
        ItemCollectedEvent,
        ItemUsedEvent,
        GameEventTriggeredEvent,
        QuestEvent
    }
}
