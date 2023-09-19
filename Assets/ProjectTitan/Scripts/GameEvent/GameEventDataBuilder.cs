using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Titan.Resource;

namespace Titan.GameEventSystem
{
    [CreateAssetMenu(fileName = "GameEventDataBuilder", menuName = "ScriptableObjects/GameEventDataBuilder")]
    public class GameEventDataBuilder : ScriptableObject
    {
        public List<GameEvent> GameEvents = new();
        public GameEvent[] GameEventArray;
        public string[] GameData;

        [ContextMenu("AddUnregistedEvents")]
        public void AddUnregistedEvents()
        {
            var guids = AssetDatabase.FindAssets("t:GameEvent");
            foreach(var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var gameEvent = AssetDatabase.LoadAssetAtPath<GameEvent>(path);
                if(GameEvents.Contains(gameEvent) == false)
                {
                    GameEvents.Add(gameEvent);
                }
            }
        }

        [ContextMenu("ShowJson")]
        public void ShowJson()
        {
            GameData = new string[GameEvents.Count];
            for(int i = 0; i < GameEvents.Count; i++)
            {
                GameData[i] = JsonUtility.ToJson(GameEvents[i], true);
            }

            Debug.Log($"Cont : {GameEvents.Count}");
            var json = JsonUtility.ToJson(this, true);
            Debug.Log(json);
        }
    }
}
