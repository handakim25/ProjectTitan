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
    public class GameEventDataBuilder : BaseData
    {
        public List<GameEventObject> GameEvents = new();
        private List<GameEvent> _gameEvent;

        private string _dataPath = "Data/";
        private string _dataFileName = "GameEventData.json";

        private string ResourceFilePath => _dataPath + _dataFileName;
        private string AbsoluteFilePath => Application.dataPath + DataDirectory + _dataPath + _dataFileName;

        public override void SaveData()
        {
            var eventList = GameEvents.Select(x => x.GameEvent).ToList();
            var data = JsonConvert.SerializeObject(eventList, Formatting.Indented);
            File.WriteAllText(AbsoluteFilePath, data); 
        }

        public override void LoadData()
        {
            TextAsset asset = ResourceManager.Load<TextAsset>(ResourceFilePath);
            if(asset == null || asset.text == null)
            {
                Debug.LogError("Game Event Data is not found");
                return;
            }

            _gameEvent = JsonConvert.DeserializeObject<List<GameEvent>>(asset.text);
        }

        public void AddData(GameEventObject gameEventObject)
        {
            GameEvents.Add(gameEventObject);
        }

        public List<GameEvent> GetGameEvents()
        {
            return _gameEvent;
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
