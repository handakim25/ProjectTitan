using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

using Titan.Resource;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game Event의 Editor Data
    /// </summary>
    public class GameEventData : BaseData
    {
        public List<GameEventObject> GameEvents = new();
        private List<GameEvent> _gameEvent;
        private Dictionary<string, GameEvent> _gameEventDictionary;

        /// <summary>
        /// Data file path. Resource 폴더 기준이다.
        /// </summary>
        private string _dataPath = "Data/";
        private string _resourcePath = "Data/GameEventData";
        private string _dataFileName = "GameEventData.json";

        private string ResourceFilePath => _dataPath + _dataFileName;
        private string AbsoluteFilePath => Application.dataPath + DataDirectory + _dataFileName;

        public override void SaveData()
        {
            var eventList = GameEvents.Select(x => x.GameEvent).ToList();
            var data = JsonConvert.SerializeObject(eventList, Formatting.Indented);
            File.WriteAllText(AbsoluteFilePath, data); 
        }

        public override void LoadData()
        {
            TextAsset asset = ResourceManager.Load<TextAsset>(_resourcePath);
            if(asset == null || asset.text == null)
            {
                Debug.Log($"Resource File Path : {ResourceFilePath}");
                Debug.LogError("Game Event Data is not found");
                return;
            }

            _gameEvent = JsonConvert.DeserializeObject<List<GameEvent>>(asset.text);
            _gameEventDictionary = _gameEvent.ToDictionary(x => x.EventName, x => x);
        }

        public void AddData(GameEventObject gameEventObject)
        {
            GameEvents.Add(gameEventObject);
        }

        public List<GameEvent> GetGameEvents()
        {
            return _gameEvent;
        }

        public GameEvent GetEvent(string eventName)
        {
            if(_gameEventDictionary.TryGetValue(eventName, out GameEvent gameEvent) == false)
            {
                Debug.LogError($"Game Event {eventName} is not found");
                return null;
            }
            return gameEvent;
        }

        public bool TryGetEvent(string eventName, out GameEvent gameEvent)
        {
            return _gameEventDictionary.TryGetValue(eventName, out gameEvent);
        }

        [ContextMenu("Show Json Data")]
        private void ShowJsonData()
        {
            var eventList = GameEvents.Select(x => x.GameEvent).ToList();
            var data = JsonConvert.SerializeObject(eventList, Formatting.Indented);
            Debug.Log($"Game Event Data : \n{data}");
        }
    }
}
