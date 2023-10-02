using System;
using System.IO;
using System.Xml;
using UnityEngine;

using Titan.Resource;
using Titan.Utility;
using System.Linq;

namespace Titan.Effects
{
    /// <summary>
    /// Effect Clip 리스트와 이펙트 파일 이름과 경로를 가지고 있다.
    /// 파일을 읽고 쓰는 기능이 있다.
    /// 일단은 XML 데이터를 이용하지만 추후에 다른 포멧으로 변경할 것
    /// </summary>
    public class EffectData : BaseData
    {
        public EffectClip[] EffectClips = new EffectClip[0];

        // Path
        private string _clipPath = "Effects/";
        protected override string ResourcePath => "Data/EffectData";
        protected override string DataFileName => "EffectData.json";

        // XML Delimeter
        private EffectData() {}

        #region XML
        
        public override void LoadData()
        {
            // Application.dataPath : <Project Folder>/Assets
            // DataDirectory : /ProjectTitan/ResourcesData/Resources/Data/
            var asset = (TextAsset)ResourceManager.Load(ResourcePath);
            if(asset == null || asset.text == null)
            {
                AddData("New Effect");
                return;
            }

            JsonUtility.FromJsonOverwrite(asset.text, this);
        }

        // WriteStartDocument / WriteEndDocument : XML 시작, 끝 형식 출력
        // WriteStartElement / WriteEndElement : XML Element 출력
        // Element 사이에는 여러 다른 Element들이 올 수 있다.
        // WriteElementString : 중간에 다른 자식이 없을 경우 사용 가능
        // WriteAttributeString : Element가 Attribute를 가질 경우 이 함수를 이용해 출력 가능

        public override void SaveData()
        {
            var data = JsonUtility.ToJson(this, true);
            File.WriteAllText(SaveFilePath + DataFileName, data);
        }

        #endregion XML

        #region Manage Data
        
        public override int AddData(string newName)
        {
            if(names == null)
            {
                names = new string[] {newName};
                EffectClips = new EffectClip[] {new EffectClip()};
            }
            else
            {
                names = names.Concat(new [] {newName}).ToArray();
                EffectClips = EffectClips.Concat(new [] {new EffectClip()}).ToArray();
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
            EffectClips = EffectClips.Where((clip, index) => index != removeIndex).ToArray();
            if(EffectClips.Length == 0)
            {
                EffectClips = null;
            }
        }

        /// <summary>
        /// 지정한 Index의 Clip을 복사하여 생성하고
        /// 리스트의 끝에 추가한다.
        /// </summary>
        /// <param name="index"></param>
        public override void Copy(int index)
        {
            names = names.Concat(new[] {names[index]}).ToArray();
            EffectClips = EffectClips.Concat(new[] {GetCopy(index)}).ToArray();
        }

        // @Refactor 리플렉션 혹은 Generic 이용
        public EffectClip GetCopy(int index)
        {
            if(index < 0 || index >= EffectClips.Length)
            {
                return null;
            }

            EffectClip origin = EffectClips[index];
            EffectClip copy = ObjectCloner.SerializeClone(origin);

            copy.PreLoad();
            return copy;
        }

        /// <summary>
        /// 원하는 index를 찾아서 preloading한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Effect Clip. 못 찾을 경우 null</returns>
        public EffectClip GetClip(int index)
        {
            if(index < 0 || index >= EffectClips.Length)
            {
                return null;
            }
            EffectClips[index].PreLoad();
            return EffectClips[index];
        }

        public override void ClearData()
        {
            foreach(EffectClip clip in EffectClips)
            {
                clip.Release();
            }
            EffectClips = null;
            names = null;
        }
        
        #endregion Manage Data
    }
}
