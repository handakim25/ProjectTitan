using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Titan.Utility;

namespace Titan.QuestSystem
{
    /// <summary>
    /// Quest의 편집 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest/Quest Object", order = 0)]
    public class QuestObject : ScriptableObject
    {
        public Quest Quest;

        // Json Test Method
        [ContextMenu("Show Json")]
        public void TestMethod()
        {
            Debug.Log("Json Utility");
            Debug.Log(JsonUtility.ToJson(Quest, true));
            Debug.Log($"Newton json");
            Debug.Log(JsonConvert.SerializeObject(Quest, Formatting.Indented));

            Debug.Log("Serialize self Test");
            Debug.Log("Json Utlity");
            Debug.Log(JsonConvert.SerializeObject(this, Formatting.Indented));
            Debug.Log($"Newton json");
            Debug.Log(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
