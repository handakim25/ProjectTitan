using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;

// @memo
// 개인적으로는 Run-time에서 사용되는 코드하고 Editor 사용 코드가 섞여 있어서 별로 같다.
// 각각 적절하게 나누는 편이 코드를 명확하게 이해할 수 있고 확장성에 더 좋다.

namespace Titan
{
    public class QuestData : BaseData
    {
        /// <summary>
        /// Quest Data를 저장해두는 파일 경로, Resources에 상대적인 경로이다.
        /// Asset 구조로 불러오기 때문에 확장자는 필요 없다.
        /// </summary>
        private string _dataPath = "Data/QuestData";
        private string _dataFilePath = "";
        /// <summary>
        /// Quest Data를 저장해두는 파일 이름
        /// </summary>
        private string _dataFileName = "QuestData.json";

        /// <summary>
        /// Only called on editor mode
        /// </summary>
        public override void SaveData()
        {
            var data = JsonUtility.ToJson(this, true);
            File.WriteAllText(_dataFilePath + _dataFileName, data);
        }

        public override void LoadData()
        {
            // 런타임 중이라면 여기서 AssetBundle에 접근해야 한다.
            _dataFilePath = Application.dataPath + DataDirectory;
            TextAsset asset = (TextAsset)Resources.Load(_dataPath, typeof(TextAsset));
            if(asset == null || asset.text == null)
            {
                AddData("New Quest");
                return;
            }

            JsonUtility.FromJsonOverwrite(asset.text, this);
        }

        public override int AddData(string newName)
        {
            if(newName == null)
            {
                names = new string[] {newName};

            }
            else
            {
                names = names.Concat(new [] {newName}).ToArray();
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
            
        }

        public override void Copy(int index)
        {
        }
    }
}
