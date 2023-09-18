using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Titan.Resource;

namespace Titan.QuestSystem
{
    [CreateAssetMenu(fileName = "QuestObjectDatabase", menuName = "Titan/QuestObjectDatabase", order = 0)]
    public class QuestObjectDatabase : BaseData
    {
        public QuestObject[] QuestObjects = new QuestObject[0];
        [System.NonSerialized] public Dictionary<string, QuestObject> _questDictionary = new();

        string _dataPath = "Data/QuestObjectData";
        string _dataFilePath = "";
        string _dataFileName = "QuestObjectData.json";

        [ContextMenu("Save to Json File")]
        public override void SaveData()
        {
            SetIndexNumbers();

            _dataFilePath = Application.dataPath + DataDirectory;
            var questList = QuestObjects.Select(quest => quest.Quest).ToList();
            string json = JsonConvert.SerializeObject(questList, Formatting.Indented);
            File.WriteAllText(_dataFilePath + _dataFileName, json);
        }

        private void SetIndexNumbers()
        {
            for (int i = 0; i < QuestObjects.Length; i++)
            {
                QuestObject questObject = QuestObjects[i];
                questObject.Quest.QuestIndex = i;
            }
        }

        public void LoadFromJson()
        {
            Debug.Log("Load Quest Data");
            TextAsset textAsset = Resources.Load<TextAsset>(_dataPath);
            if(textAsset == null || textAsset.text == null)
            {
                return;
            }
            string json = textAsset.text;
            var questList = JsonConvert.DeserializeObject<List<Quest>>(json);
            QuestObjects = questList.Select(quest => {
                var questObject = CreateInstance<QuestObject>();
                questObject.Quest = quest;
                return questObject;
            }).ToArray();
            names = QuestObjects.Select(quest => quest.Quest.QuestID).ToArray();
            _questDictionary = QuestObjects.ToDictionary(quest => quest.Quest.QuestID);;
        }

        public Quest GetQuest(string questID)
        {
            if(!_questDictionary.ContainsKey(questID))
            {
                return null;
            }
            return _questDictionary[questID].Quest;
        }

        public bool TryGetQuest(string questID, out Quest quest)
        {
            quest = GetQuest(questID);
            return quest != null;
        }

        [ContextMenu("Show Saved Json")]
        public void ShowJson()
        {
            var questList = QuestObjects.Select(quest => quest.Quest).ToList();
            string json = JsonConvert.SerializeObject(questList, Formatting.Indented);
            Debug.Log(json);
        }
    }
}
