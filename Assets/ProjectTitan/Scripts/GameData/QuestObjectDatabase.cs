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
    /// <summary>
    /// Quest Database이다. Scriptable Object로 Asset으로 존재하지만 실제 데이터는 Json으로 저장되어 있다.
    /// </summary>
    [CreateAssetMenu(fileName = "QuestObjectDatabase", menuName = "Quest/QuestObjectDatabase", order = 0)]
    public class QuestObjectDatabase : BaseData
    {
        public QuestObject[] QuestObjects = new QuestObject[0];
        [System.NonSerialized] public Dictionary<string, QuestObject> _questDictionary = new();

        protected override string ResourcePath => "Data/QuestObjectData";
        protected override string DataFileName => "QuestObjectData.json";

        /// <summary>
        /// Json 파일로 변환해서 저장한다. 에디터 툴을 따로 작성하지 않았기 때문에 ContextMenu를 이용해서 저장한다.
        /// </summary>
        [ContextMenu("Save to Json File")]
        public override void SaveData()
        {
            SetIndexNumbers(QuestObjects);

            var questList = QuestObjects.Select(quest => quest.Quest).ToList();
            string json = JsonConvert.SerializeObject(questList, Formatting.Indented);
            File.WriteAllText(SaveFilePath + DataFileName, json);
        }

        /// <summary>
        /// Quest들의 Index 버노를 설정한다.
        /// </summary>
        private void SetIndexNumbers(QuestObject[] questObjects)
        {
            for (int i = 0; i < questObjects.Length; i++)
            {
                QuestObject questObject = QuestObjects[i];
                questObject.Quest.QuestIndex = i;
            }
        }

        public void LoadFromJson()
        {
            Debug.Log("Load Quest Data");
            TextAsset textAsset = Resources.Load<TextAsset>(ResourcePath);
            if(textAsset == null || textAsset.text == null)
            {
                Debug.LogError($"Cannot load Quest Data from {ResourcePath}");
                return;
            }
            string json = textAsset.text;
            var questList = JsonConvert.DeserializeObject<List<Quest>>(json);
            QuestObjects = questList.Select(quest => {
                var questObject = CreateInstance<QuestObject>();
                questObject.Quest = quest;
                return questObject;
            }).ToArray();
            names = QuestObjects.Select(quest => quest.Quest.QuestID).ToArray(); // backward-compatible
            _questDictionary = QuestObjects.ToDictionary(quest => quest.Quest.QuestID);;
        }

        /// <summary>
        /// Quest ID로 Quest를 찾는다.
        /// </summary>
        /// <param name="questID">없을 경우 null을 반환</param>
        /// <returns></returns>
        public Quest GetQuest(string questID)
        {
            if(!_questDictionary.ContainsKey(questID))
            {
                return null;
            }
            return _questDictionary[questID].Quest;
        }

        /// <summary>
        /// Quest ID로 Quest를 찾는다.
        /// </summary>
        /// <param name="questID">찾을 Quest ID</param>
        /// <param name="quest">반환값</param>
        /// <returns></returns>
        public bool TryGetQuest(string questID, out Quest quest)
        {
            quest = GetQuest(questID);
            return quest != null;
        }

        public bool Contains(string questID)
        {
            if(string.IsNullOrEmpty(questID))
            {
                return false;
            }
            return _questDictionary.ContainsKey(questID);
        }

        /// <summary>
        /// Debug용 Json을 보여준다.
        /// </summary>
        [ContextMenu("Show Saved Json")]
        public void ShowJson()
        {
            var questList = QuestObjects.Select(quest => quest.Quest).ToList();
            string json = JsonConvert.SerializeObject(questList, Formatting.Indented);
            Debug.Log(json);
        }
    }
}
