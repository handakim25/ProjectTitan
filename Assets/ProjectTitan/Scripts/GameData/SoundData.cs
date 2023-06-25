using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;

namespace Titan.Audio
{
    /// <summary>
    /// Sound 데이터들을 저장하고 관리한다
    /// </summary>
    public class SoundData : BaseData
    {
        public SoundClip[] SoundClips = new SoundClip[0];

        // Path
        private string _clipPath = "Sound/";
        private string _dataPath = "Data/soundData";
        private string _xmlFilePath = "";
        private string _xmlFileName = "soundData.xml";

        // XML Delimeter
        private static string SOUND = "sound";
        private static string CLIP = "clip";

        public SoundData() {}

        public override void SaveData()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.Unicode;
            using(XmlWriter xml = XmlWriter.Create(_xmlFilePath + _xmlFileName, settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement(SOUND); // SOUND Start
                xml.WriteElementString("length", Count.ToString());
                for(int i = 0; i < Count; ++i)
                {
                    SoundClip clip = SoundClips[i];
                    xml.WriteStartElement(CLIP); // CLIP Start
                    xml.WriteElementString("ID", i.ToString());
                    xml.WriteElementString("Name", names[i]);
                    xml.WriteElementString("SoundName", clip.clipName);
                    xml.WriteElementString("SoundPath", clip.clipPath);
                    xml.WriteElementString("PlayType", clip.playType.ToString());
                    xml.WriteElementString("MaxVolume", clip.maxVolume.ToString());
                    xml.WriteElementString("Pitch", clip.pitch.ToString());
                    xml.WriteElementString("DopplerLevel", clip.dopplerLevel.ToString());
                    xml.WriteElementString("RollOffMode", clip.rolloffMode.ToString());
                    xml.WriteElementString("MinDistance", clip.minDistance.ToString());
                    xml.WriteElementString("MaxDistance", clip.maxDistance.ToString());
                    xml.WriteElementString("SparialBlend", clip.sparialBlend.ToString());
                    if(clip.IsLoop)
                    {
                        xml.WriteElementString("loop", "true");
                    }
                    xml.WriteElementString("CheckTimeCount", clip.checkTime.Length.ToString());
                    string checkTime = "";
                    foreach(float t in clip.checkTime)
                    {
                        checkTime += $"{t.ToString()}/";
                    }
                    xml.WriteElementString("CheckTime", checkTime);
                    string setTime = "";
                    foreach(float t in clip.setTime)
                    {
                        setTime += $"{t.ToString()}/";
                    }
                    xml.WriteElementString("SetTime", setTime);

                    xml.WriteEndElement(); // CLIP End
                }
                xml.WriteEndElement(); // SOUND End
                xml.WriteEndDocument();
            }
        }

        public override void LoadData()
        {
            _xmlFilePath = Application.dataPath + DataDirectory;
            TextAsset asset = (TextAsset)Resources.Load(_dataPath, typeof(TextAsset));
            if(asset == null || asset.text == null)
            {
                AddData("New Sound");
                return;
            }

            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using(XmlReader xml = XmlReader.Create(new StringReader(asset.text)))
            {
                while(xml.Read())
                {
                    if(xml.IsStartElement(SOUND))
                    {
                        xml.ReadStartElement();

                        int length = xml.ReadElementContentAsInt();
                        names = new string[length];
                        SoundClips = new SoundClip[length];
                    }
                    if(xml.IsStartElement(CLIP))
                    {
                         
                    }
                }
            }
        }
    }
}
