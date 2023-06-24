using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Titan.Effects;

using UnityObject = UnityEngine.Object;

namespace Titan.Resource
{
    public class EffectTool : EditorWindow
    {
        // Unity Pixel size
        public int uiWidthLarge = 300;
        public int uiWidthMiddle = 200;
        private int selection = 0; 
        private Vector2 listScrollPos = Vector2.zero;
        private Vector2 contentScrollPos = Vector2.zero;

        private GameObject effectSource = null; // 현재 선택한 데이터
        private static EffectData effectData;

        private const string EnumName = "EffectList";
        [MenuItem("Tools/Effect Tool")]
        static void Init()
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();

            EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
            window.Show();
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        private void OnGUI()
        {
            if(effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }

            EditorGUILayout.BeginVertical();
            {
                // 상단바
                UnityObject source = effectSource;
                EditorHelper.EditorToolTopLayer(effectData, ref selection, ref source, uiWidthMiddle);
                effectSource = (GameObject)source;

                EditorGUILayout.BeginHorizontal();
                {
                    // 좌측 리스트
                    EditorHelper.EditorToolListLayer(effectData, ref listScrollPos, ref selection, ref source, uiWidthLarge);
                    effectSource = (GameObject)source;

                    // Body
                    EditorToolBodyLayer();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Reload Settings"))
                {
                    effectData = CreateInstance<EffectData>();
                    effectData.LoadData();
                    selection = 0;
                    effectSource = null;
                }
                if(GUILayout.Button("Save"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void EditorToolBodyLayer()
        {
            EditorGUILayout.BeginVertical();
            {
                contentScrollPos = EditorGUILayout.BeginScrollView(contentScrollPos);
                {
                    if(effectData.Count > 0)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.Separator(); 
                            EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                            effectData.names[selection] = EditorGUILayout.TextField("Name", effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                            var curClip = effectData.EffectClips[selection];
                            curClip.effectType = (EffectType)EditorGUILayout.EnumPopup("Effect Type", curClip.effectType, GUILayout.Width(uiWidthLarge));

                            EditorGUILayout.Separator();
                            if(effectSource == null && curClip.effectName != string.Empty)
                            {
                                curClip.PreLoad();
                                effectSource = Resources.Load(curClip.effectPath + curClip.effectName) as GameObject;
                            }
                            // Object를 끌어다 쓸 수 있는 필드
                            effectSource = (GameObject)EditorGUILayout.ObjectField("Effect", effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f));
                            if(effectSource != null)
                            {
                                curClip.effectPath = EditorHelper.GetPath(effectSource);
                                curClip.effectName = effectSource.name;
                            }
                            else
                            {
                                curClip.effectPath = string.Empty;
                                curClip.effectName = string.Empty;
                                effectSource = null;
                            }
                            
                            EditorGUILayout.Separator();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        public void CreateEnumStructure()
        {
            var builder = new StringBuilder();        
            builder.AppendLine();
            for(int i = 0; i < effectData.Count; ++i)
            {
                if(effectData.names[i] != string.Empty)
                {
                    // @Refactor
                    // 공백 이름 대응할 수 있도록 수정
                    builder.AppendLine($"\t{effectData.names[i].Replace(' ', '_')} = {i},");
                }
            }
            EditorHelper.CreateEnumStructure(EnumName, builder);
        }

        private bool HasDuplicateName()
        {
            return effectData.names.Length != effectData.names.Distinct().Count();
        }

        private void Save()
        {
            if(HasDuplicateName())
            {
                Debug.LogError($"Has Duplicate Name");
                List<string> duplicates = effectData.names
                    .GroupBy(s => s) // string을 그룹으로 묶는데
                    .Where(g => g.Count() > 1) // count가 1 이상인 것들을 묶는다
                    .Select(g => g.Key) // 그룹에 묶은 것의 key를 선택
                    .ToList();
                foreach(string duplicate in duplicates)
                {
                    Debug.Log($"Duplicate : {duplicate}");
                }
                return;
            }
            effectData.SaveData();
            CreateEnumStructure();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
