using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;
using Titan.Utility;

// @memo
// 개인적으로는 Run-time에서 사용되는 코드하고 Editor 사용 코드가 섞여 있어서 별로 같다.
// 각각 적절하게 나누는 편이 코드를 명확하게 이해할 수 있고 확장성에 더 좋다.

namespace Titan.QuestSystem
{
    /// <summary>
    /// 현재 사용되지 않는다.
    /// </summary>
    public class QuestDatabase : BaseData
    {
        public Quest[] Quests = new Quest[0];

        /// <summary>
        /// Quest Data를 저장해두는 파일 경로, Resources에 상대적인 경로이다.
        /// Asset 구조로 불러오기 때문에 확장자는 필요 없다.
        /// </summary>
        private string _dataPath = "Data/QuestData";
        private string _dataFileName = "QuestData.json";

        /// <summary>
        /// Only called on editor mode
        /// </summary>
        public override void SaveData()
        {
            var data = JsonUtility.ToJson(this, true);
            File.WriteAllText(SaveFilePath + _dataFileName, data);
        }

        public override void LoadData()
        {
            // 런타임 중이라면 여기서 AssetBundle에 접근해야 한다.
            TextAsset asset = (TextAsset)Resources.Load(_dataPath, typeof(TextAsset));
            if(asset == null || asset.text == null)
            {
                AddData("New Quest");
                return;
            }

            JsonUtility.FromJsonOverwrite(asset.text, this);
        }

        /// <summary>
        /// new name인 Quest를 추가한다.
        /// </summary>
        /// <param name="newName">새로 추가할 퀘스트 이름</param>
        /// <returns>현재 퀘스트 Count</returns>
        public override int AddData(string newName)
        {
            if(names == null)
            {
                names = new string[] {newName};
                Quests = new Quest[] {new()};
            }
            else
            {
                names = names.Concat(new [] {newName}).ToArray();
                Quests = Quests.Concat(new [] {new Quest()}).ToArray();
            }
            return Count;
        }

        public override void RemoveData(int removeIndex)
        {
            names = names.Where((name, index) => index != removeIndex).ToArray();
            if(names.Length == 0)
            {
                names = null;
            }
            Quests = Quests.Where((quest, index) => index != removeIndex).ToArray();
            if(Quests.Length == 0)
            {
                Quests = null;
            }
        }

        public override void Copy(int index)
        {
            names = names.Concat(new[] {names[index]}).ToArray();
            Quests = Quests.Concat(new[] {GetCopy(index)}).ToArray();
        }

        public Quest GetCopy(int index)
        {
            if(index < 0 || index >= Quests.Length)
            {
                return null;
            }

            var origin = Quests[index];
            var copy = ObjectCloner.SerializeClone(origin);
            return copy;
        }
    }
}
