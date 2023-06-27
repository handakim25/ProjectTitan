using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Titan.Audio;

using UnityObject = UnityEngine.Object;

namespace Titan.Resource
{
    public class SoundTool : EditorWindow
    {
        public int uiWidthLarge = 450;
        public int uiWidthMiddle = 300;
        public int uiWidthSmall = 200;
    
        private int selection = 0;
        private Vector2 _listScrollPos = Vector2.zero;
        private Vector2 _contentScrollPos = Vector2.zero;
    
        private AudioClip soundSource;
        private static SoundData _soundData;

        private const string EnumName = "SoundList";

        [MenuItem("Tools/Sound Tool")]
        static void Init()
        {
            _soundData = CreateInstance<SoundData>();
            _soundData.LoadData();

            SoundTool window = GetWindow<SoundTool>(false, "Sound Tool");
            window.Show();
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        private void OnGUI()
        {
            if(_soundData == null)
            {
                _soundData = ScriptableObject.CreateInstance<SoundData>();
                _soundData.LoadData();
            }

            EditorGUILayout.BeginVertical();
            {
                UnityObject source = soundSource;
                EditorHelper.EditorToolTopLayer(_soundData, ref selection, ref source, uiWidthMiddle);
                soundSource = (AudioClip)source;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorHelper.EditorToolListLayer(_soundData, ref _listScrollPos, ref selection, ref source, uiWidthMiddle);
                    soundSource = (AudioClip)source;

                    EditorGUILayout.BeginVertical();
                    {
                        _listScrollPos = EditorGUILayout.BeginScrollView(_listScrollPos);
                        {
                            if(_soundData.Count > 0)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Separator();
                                    SoundClip curClip = _soundData.SoundClips[selection];

                                    EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                    _soundData.names[selection] = EditorGUILayout.TextField("Name", _soundData.names[selection], GUILayout.Width(uiWidthLarge));
                                    curClip.playType = (SoundPlayType)EditorGUILayout.EnumPopup("PlayType", curClip.playType, GUILayout.Width(uiWidthLarge));
                                    curClip.maxVolume = EditorGUILayout.FloatField("Max Volume", curClip.maxVolume, GUILayout.Width(uiWidthLarge));
                                    curClip.IsLoop = EditorGUILayout.Toggle("Loop Clip", curClip.IsLoop, GUILayout.Width(uiWidthLarge));

                                    // 이거 Path가 변경될 경우는 어떻게 되지?
                                    EditorGUILayout.Separator();
                                    if(soundSource == null && curClip.clipName != string.Empty)
                                    {
                                        soundSource = Resources.Load(curClip.clipPath + curClip.clipName) as AudioClip;
                                    }
                                    soundSource = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", soundSource, typeof(AudioClip), false, GUILayout.Width(uiWidthLarge));

                                    if(soundSource != null)
                                    {
                                        curClip.clipPath = EditorHelper.GetPath(soundSource);
                                        curClip.clipName = soundSource.name;
                                        curClip.pitch = EditorGUILayout.Slider("Pitch", curClip.pitch, -3.0f, 3.0f, GUILayout.Width(uiWidthLarge));
                                        curClip.dopplerLevel = EditorGUILayout.Slider("Doppler", curClip.dopplerLevel, 0.0f, 5.0f, GUILayout.Width(uiWidthLarge));
                                        curClip.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", curClip.rolloffMode, GUILayout.Width(uiWidthLarge));
                                        curClip.minDistance = EditorGUILayout.FloatField("Min Distance", curClip.minDistance, GUILayout.Width(uiWidthLarge));
                                        curClip.maxDistance = EditorGUILayout.FloatField("Max Distance", curClip.maxDistance, GUILayout.Width(uiWidthLarge));
                                        curClip.spatialBlend = EditorGUILayout.Slider("Pan Level", curClip.spatialBlend, 0.0f, 1.0f, GUILayout.Width(uiWidthLarge));
                                    }
                                    else
                                    {
                                        curClip.clipName = string.Empty;
                                        curClip.clipPath = string.Empty;
                                    }

                                    EditorGUILayout.Separator();
                                    if(GUILayout.Button("Add Loop", GUILayout.Width(uiWidthMiddle)))
                                    {
                                        curClip.AddLoop();
                                    }
                                    // 따로 분리할 것
                                    for(int i = 0; i < curClip.checkTime.Length; i++)
                                    {
                                        EditorGUILayout.BeginVertical("Box");
                                        {
                                            GUILayout.Label($"Loop Step {i}", EditorStyles.boldLabel);
                                            if(GUILayout.Button("Remoive", GUILayout.Width(uiWidthMiddle)))
                                            {
                                                curClip.RemoveLoop(i);
                                                EditorGUILayout.EndVertical(); // Box End
                                                continue;
                                            }
                                            curClip.checkTime[i] = EditorGUILayout.FloatField("Check Time", curClip.checkTime[i], GUILayout.Width(uiWidthMiddle));
                                            curClip.setTime[i] = EditorGUILayout.FloatField("Set Time", curClip.setTime[i], GUILayout.Width(uiWidthMiddle));
                                        }
                                        EditorGUILayout.EndVertical(); // Box End
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Reload"))
                {
                    _soundData = CreateInstance<SoundData>();
                    _soundData.LoadData();
                    selection = 0;
                    soundSource = null;
                }
                if(GUILayout.Button("Save"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Save()
        {
            if(HasDuplicateName())
            {
                Debug.LogError($"Has Duplicate Name");
                List<string> duplicates = _soundData.names
                    .GroupBy(s => s)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                foreach(string duplicate in duplicates)
                {
                    Debug.Log($"Duplicate : {duplicate}");
                }
                return;
            }   
            _soundData.SaveData();
            CreateEnumStructure();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private void CreateEnumStructure()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < _soundData.Count; i++)
            {
                if(_soundData.names[i] != string.Empty)
                {
                    builder.AppendLine($"    {_soundData.names[i].Replace(' ', '_')} = {i}");
                }
            }
            EditorHelper.CreateEnumStructure(EnumName, builder);
        }

        private bool HasDuplicateName()
        {
            return _soundData.names.Length != _soundData.name.Distinct().Count();
        }
    }
}