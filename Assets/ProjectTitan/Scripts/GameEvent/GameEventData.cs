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
    /// Game Event의 Data를 관리하는 클래스. Game Event의 Data는 Json 파일로 저장되어 있다.
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
        protected override string ResourcePath => "Data/GameEventData";
        protected override string DataFileName => "GameEventData.json";

        /// <summary>
        /// Editor에서 호출되는 Class. Game Event Data를 Json 파일로 저장한다.
        /// </summary>
        public override void SaveData()
        {
            var eventList = GameEvents.Select(x => x.GameEvent).ToList();
            var data = JsonConvert.SerializeObject(eventList, Formatting.Indented);
            File.WriteAllText(SaveFilePath + DataFileName, data); 
        }

        /// <summary>
        /// Editor 혹은 Run-Time에서 Data를 Load. Load 시에 ID를 기반으로 Dictionary를 생성
        /// </summary>
        public override void LoadData()
        {
            TextAsset asset = ResourceManager.Load<TextAsset>(ResourcePath);
            if(asset == null || asset.text == null)
            {
                Debug.LogError("Game Event Data is not found");
                return;
            }

            _gameEvent = JsonConvert.DeserializeObject<List<GameEvent>>(asset.text);
            _gameEventDictionary = _gameEvent.ToDictionary(x => x.EventName, x => x);
        }

        /// <summary>
        /// Editor에서 호출되는 Class. Game Event Data를 추가한다.
        /// </summary>
        /// <param name="gameEventObject"></param>
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

        /// <summary>
        /// Test를 위한 함수. 현재 JSON Data를 Log로 출력한다.
        /// </summary>
        [ContextMenu("Show Json Data")]
        private void ShowJsonData()
        {
            var eventList = GameEvents.Select(x => x.GameEvent).ToList();
            var data = JsonConvert.SerializeObject(eventList, Formatting.Indented);
            Debug.Log($"Game Event Data : \n{data}");
        }
    }
}
