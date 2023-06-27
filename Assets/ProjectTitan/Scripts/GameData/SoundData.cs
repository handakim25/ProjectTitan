using System.Xml;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;

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
                    xml.WriteElementString("SpatialBlend", clip.spatialBlend.ToString());
                    if(clip.IsLoop)
                    {
                        xml.WriteElementString("Loop", "true");
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
                        xml.ReadStartElement(); // Sound Start

                        int length = xml.ReadElementContentAsInt();
                        names = new string[length];
                        SoundClips = new SoundClip[length];
                    
                        // @Refactor
                        // 구조가 유연하지 못하다. Save Format에 유연하게 작동하도록 개선할 것
                        while(xml.IsStartElement(CLIP))
                        {
                            xml.ReadStartElement(); // Clip Start

                            int curId = xml.ReadElementContentAsInt("ID", "");
                            names[curId] = xml.ReadElementContentAsString("Name", "");
                            SoundClips[curId] = new SoundClip() {index = curId};

                            SoundClip curClip = SoundClips[curId];
                            curClip.clipName = xml.ReadElementContentAsString("SoundName", "");
                            curClip.clipPath = xml.ReadElementContentAsString("SoundPath", "");
                            curClip.playType = System.Enum.Parse<SoundPlayType>(xml.ReadElementContentAsString("PlayType", ""));
                            curClip.maxVolume = xml.ReadElementContentAsFloat("MaxVolume", "");
                            curClip.pitch = xml.ReadElementContentAsFloat("Pitch", "");
                            curClip.dopplerLevel = xml.ReadElementContentAsFloat("Doppl!erLevel", "");
                            curClip.rolloffMode = System.Enum.Parse<AudioRolloffMode>(xml.ReadElementContentAsString("RollOffMode", ""));
                            curClip.minDistance = xml.ReadElementContentAsFloat("MinDistance", "");
                            curClip.maxDistance = xml.ReadElementContentAsFloat("MaxDistance", "");
                            curClip.spatialBlend = xml.ReadElementContentAsFloat("SpatialBlend", "");
                            if(xml.Name == "Loop")
                            {
                                curClip.IsLoop = xml.ReadElementContentAsBoolean("Loop", "");
                            }
                            int checkTimeCount = xml.ReadElementContentAsInt("CheckTimeCount", "");
                            curClip.checkTime = new float[checkTimeCount];
                            curClip.setTime = new float[checkTimeCount];

                            string checkTime = xml.ReadElementContentAsString("CheckTime", "");
                            SetLoopTime(ref curClip.checkTime, checkTime);
                            string setTime = xml.ReadElementContentAsString("SetTime", "");
                            SetLoopTime(ref curClip.setTime, setTime);

                            xml.ReadEndElement(); // Clip End
                        }
                        xml.ReadEndElement(); // Sound End
                    }
                }
            }
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
            SoundClip copy = new SoundClip();

            copy.index = SoundClips.Length;
            copy.clipName = origin.clipName;
            copy.clipPath = origin.clipPath;
            copy.playType = origin.playType;
            copy.maxVolume = origin.maxVolume;
            copy.IsLoop = origin.IsLoop;
            copy.pitch = origin.pitch;
            copy.dopplerLevel = origin.dopplerLevel;
            copy.rolloffMode = origin.rolloffMode;
            copy.minDistance = origin.minDistance;
            copy.maxDistance = origin.maxDistance;
            copy.spatialBlend = origin.spatialBlend;

            copy.setTime = new float[origin.setTime.Length];
            copy.checkTime = new float[origin.checkTime.Length];
            for(int i = 0; i < origin.setTime.Length; ++i)
            {
                copy.setTime[i] = origin.setTime[i];
                copy.checkTime[i] = origin.checkTime[i];
            }

            copy.PreLoad();
            return copy;
        }
    }
}
