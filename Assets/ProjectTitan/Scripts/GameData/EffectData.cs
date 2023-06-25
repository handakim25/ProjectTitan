using System;
using System.IO;
using System.Xml;
using UnityEngine;

using Titan.Resource;
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
        private string _dataPath = "Data/effectData";
        private string _xmlFilePath = "";
        private string _xmlFileName = "effectData.xml";

        // XML Delimeter
        private const string EFFECT = "effect";
        private const string CLIP = "clip"; 

        private EffectData() {}

        #region XML
        
        // 근데 다 좋은데 포맷 호환성 문제가 너무 심하다.

        // XML Reference
        // https://www.csharpstudy.com/Data/Xml-rw.aspx

        // XML Format
        // <Effect>
        //  <length>num(DataCount)</length>
        //  <Clip> -> n개
        //      <ID>id</ID>
        //      <Name>name</name>
        //      <EffectType>EffectType</EffectType>
        //      <EffectPath>EffectPath</EffectPath>
        //      <Effectname>EffectName</EffectName>
        //  </Clip>
        //  </Effect>

        // forward only 방식으로 동작한다.
        // Read : XML의 첫 노드로 이동한다. XML의 끝에 도달하면 False를 반환한다.
        // GetAttribute : Element의 Attribute를 읽기 위해 사용
        // ReadElementContentAs - Int, String : Element를 읽기 위해서 사용한다. 읽고 다음 노드로 이동
        // IsStartElement : Element의 시작을 확인하는 함수. 다음으로 안 넘어가기 때문에 직접 읽어주어야 한다.

        // @Refactor
        // 지금은 형식 변경에 너무 취약하다. 데이터 포맷은 자주 바뀔 수 있기 때문에
        // 조금 더 유연한 구조로 만들 필요가 있다.
        public override void LoadData()
        {
            // Application.dataPath : <Project Folder>/Assets
            // DataDirectory : /ProjectTitan/ResourcesData/Resources/Data/
            _xmlFilePath = Application.dataPath + DataDirectory;
            var asset = (TextAsset)ResourceManager.Load(_dataPath);
            if(asset == null || asset.text == null)
            {
                AddData("New Effect");
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader xml = XmlReader.Create(new StringReader(asset.text), settings))
            {
                while(xml.Read())
                {
                    if(xml.IsStartElement(EFFECT)) // IsStart는 확인만 하는 것이라서 읽고 넘어가야 한다
                    {
                        xml.ReadStartElement(); // Effect Start

                        int length = xml.ReadElementContentAsInt();
                        names = new string[length];
                        EffectClips = new EffectClip[length];

                        while(xml.IsStartElement(CLIP))
                        {
                            xml.ReadStartElement(); // Clip Start
                            int curID = xml.ReadElementContentAsInt("ID", "");
                            EffectClips[curID] = new EffectClip() {index = curID};
                            names[curID] = xml.ReadElementContentAsString("Name", "");
                            EffectClips[curID].effectType = Enum.Parse<EffectType>(xml.ReadElementContentAsString("EffectType", ""));
                            EffectClips[curID].effectPath = xml.ReadElementContentAsString("EffectPath", "");
                            EffectClips[curID].effectName = xml.ReadElementContentAsString("EffectName", "");
                            xml.ReadEndElement(); // Clip End
                        }

                        xml.ReadEndElement(); // Effect End
                    }
                }
            }
        }

        // WriteStartDocument / WriteEndDocument : XML 시작, 끝 형식 출력
        // WriteStartElement / WriteEndElement : XML Element 출력
        // Element 사이에는 여러 다른 Element들이 올 수 있다.
        // WriteElementString : 중간에 다른 자식이 없을 경우 사용 가능
        // WriteAttributeString : Element가 Attribute를 가질 경우 이 함수를 이용해 출력 가능

        public override void SaveData()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.Unicode;
            using(XmlWriter xml = XmlWriter.Create(_xmlFilePath + _xmlFileName, settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement(EFFECT); // Effect Start
                xml.WriteElementString("length", Count.ToString());
                for(int i = 0; i < Count; ++i)
                {
                    EffectClip clip = EffectClips[i];
                    xml.WriteStartElement(CLIP); // CLIP Start
                    xml.WriteElementString("ID", i.ToString());
                    xml.WriteElementString("Name", names[i]);
                    xml.WriteElementString("EffectType", clip.effectType.ToString());
                    xml.WriteElementString("EffectPath", clip.effectPath);
                    xml.WriteElementString("EffectName", clip.effectName);
                    xml.WriteEndElement(); // CLIP End
                }
                xml.WriteEndElement(); // Effect End
                xml.WriteEndDocument();
            }
        }

        #endregion XML

        #region Manage Data
        
        public override int AddData(string newName)
        {
            if(names == null)
            {
                names = new string[] {name};
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
            EffectClip copy = new EffectClip();
            copy.index = EffectClips.Length;
            copy.effectType = origin.effectType;
            copy.effectName = origin.effectName;
            copy.effectPath = origin.effectPath;
            copy.effectFullPath = origin.effectFullPath;

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
