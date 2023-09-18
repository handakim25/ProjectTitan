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
    }

    public struct ItemCollectedEvent : IEvent
    {
        public string ItemID;
        public int Count;
    }

    public struct GameEventTriggeredEvent : IEvent
    {
        public string EventName;
        
    }

    public struct QuestEvent : IEvent
    {
        public string QuestID;
        public QuestSystem.QuestStatus Status;
    }
}
