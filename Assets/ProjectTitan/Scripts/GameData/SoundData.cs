using System.Xml;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;
using Titan.Utility;

namespace Titan.Audio
{
    // @Refactor
    // SoundData, EffectData에 유사한 구조가 많다.
    // BaseData로 옮기는 것을 고려할 것
    /// <summary>
    /// Sound 데이터들을 저장하고 관리한다
    /// </summary>
    public class SoundData : BaseData
    {
        public SoundClip[] SoundClips = new SoundClip[0];

        // Path
        private string _clipPath = "Sound/";
        private string _dataPath = "Data/soundData";
        private string _dataFilePath = "";
        private string _dataFileName = "soundData.json";

        public SoundData() {}

        public override void SaveData()
        {
            var data = JsonUtility.ToJson(this, true);
            File.WriteAllText(_dataFilePath + _dataFileName, data);
        }

        public override void LoadData()
        {
            _dataFilePath = Application.dataPath + DataDirectory;
            TextAsset asset = (TextAsset)Resources.Load(_dataPath, typeof(TextAsset));
            if(asset == null || asset.text == null)
            {
                AddData("New Sound");
                return;
            }

            JsonUtility.FromJsonOverwrite(asset.text, this);
        }

        private void SetLoopTime(ref float[] target, string timeString)
        {
            string[] times = timeString.Split('/');
            for(int i = 0; i < times.Length; ++i)
            {
                if(times[i] != string.Empty)
                {
                    target[i] = float.Parse(times[i]);
                }
            }
        }

        // Prev Condition : XML is on Clip Element
        // End Condition : End of Clip
        private SoundClip ReadClip(XmlReader xml)
        {
            throw new System.NotImplementedException();
        }

        public override int AddData(string newName)
        {
            if(names == null)
            {
                names = new string[] {newName};
                SoundClips = new SoundClip[] {new SoundClip()};
            }
            else
            {
                names = names.Concat(new [] {newName}).ToArray();
                SoundClips = SoundClips.Concat(new [] {new SoundClip()}).ToArray();
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
            SoundClips = SoundClips.Where((names, index) => index != removeIndex).ToArray();
            if(SoundClips.Length == 0)
            {
                SoundClips = null;
            }
        }

        public override void Copy(int index)
        {
            names = names.Concat(new [] {names[index]}).ToArray();
            SoundClips = SoundClips.Concat(new[] {GetCopy(index)}).ToArray();
        }

        public SoundClip GetCopy(int index)
        {
            if(index < 0 || index >= SoundClips.Length)
            {
                return null;
            }

            SoundClip origin = SoundClips[index];
            SoundClip copy = ObjectCloner.SerializeClone(origin);

            copy.PreLoad();
            return copy;
        }
    }
}
