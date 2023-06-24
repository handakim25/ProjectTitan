using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace Titan.Resource
{
    // @Refactor
    // SO로 설정 가능하게 만들면 어떨까?
    public static class EditorHelper
    {
        /// <summary>
        /// 경로 계산 함수.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns>Resources 디렉토리로부터 상대 경로를 반환한다. 못 찾거나 Resources 폴더가 아닐 경우 Empty 반환. 가장 마지막에 /가 들어간다.</returns>
        public static string GetPath(UnityObject clip)
        {
            string path = AssetDatabase.GetAssetPath(clip);       
            string[] path_node = path.Split('/');
            bool findResource = false;
            string retString = string.Empty;
            for(int i = 0; i < path_node.Length - 1; ++i) // Exclude file name
            {
                if(findResource == false)
                {
                    if(path_node[i] == "Resources")
                    {
                        findResource = true;
                        retString = string.Empty;
                    }
                }
                else
                {
                    retString += path_node[i] + "/";
                }
            }     

            return retString;
        }

        /// <summary>
        /// Data 리스트를 Enum structure로 출력해주는 함수
        /// </summary>
        /// <param name="enumName">enum file name</param>
        /// <param name="data">enum 데이터, tab을 넣어주어야 한다</param>
        public static void CreateEnumStructure(string enumName, StringBuilder data)
        {
            string tempateFilePath = "Assets/ProjectTitan/Editor/EnumTemplate.txt";
            if(!File.Exists(tempateFilePath))
            {   
                Debug.LogError($"Missing Template File: {tempateFilePath}");
                return;
            }
            
            string entityTemplate = File.ReadAllText(tempateFilePath);
            entityTemplate = entityTemplate.Replace("$DATA$", data.ToString());
            entityTemplate = entityTemplate.Replace("$ENUM$", enumName);

            string folderPath = "Assets/ProjectTitan/Scripts/GameData/";
            if(Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = folderPath + enumName + ".cs";
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, entityTemplate);
        }

        /// <summary>
        /// Editor들이 공통적으로 사용하는 상단바. Horizontal Layout
        /// Copy 혹은 Remove 시에 Source가 null이 된다
        /// </summary>
        /// <param name="data">현재 편집하고 있는 Data</param>
        /// <param name="selection">현재 선택하고 있는 index, 0-base</param>
        /// <param name="source"></param>
        /// <param name="uiWidth">UI 기준 크기</param>
        public static void EditorToolTopLayer(BaseData data, ref int selection, ref UnityObject source, int uiWidth)
        {
            EditorGUILayout.BeginHorizontal(); // Start 최상위
            {
                if(GUILayout.Button("Add", GUILayout.Width(uiWidth)))
                {
                    data.AddData("NewData");
                    selection = data.Count - 1; // 최종 리스트를 선택
                }
                if(GUILayout.Button("Copy", GUILayout.Width(uiWidth)))
                {
                    data.Copy(selection);
                    source = null;
                    selection = data.Count - 1;
                }
                if(data.Count > 1 && GUILayout.Button("Remove",GUILayout.Width(uiWidth)))
                {
                    source = null;
                    data.RemoveData(selection);
                }

                if(selection > data.Count - 1)
                {
                    selection = data.Count - 1;
                }
            }
            EditorGUILayout.EndHorizontal(); // End 최상위 
        }

        /// <summary>
        /// 좌측에 들어갈 스크롤뷰. 선택이 바뀔 경우 source가 null이 된다. Vertical Layout
        /// </summary>
        /// <param name="data">표시할 데이터 목록</param>
        /// <param name="scrollPosition"></param>
        /// <param name="selection">선택 인덱스. 0-base</param>
        /// <param name="source"></param>
        /// <param name="uiWidth"></param>
        public static void EditorToolListLayer(BaseData data, ref Vector2 scrollPosition, ref int selection, ref UnityObject source, int uiWidth)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical("box");
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    {
                        if(data.Count > 0)
                        {
                            int lastSelection = selection;
                            selection = GUILayout.SelectionGrid(selection, data.GetNameList(true), 1); // 한 줄 그리드 생성
                            if(lastSelection != selection)
                            {
                                source = null;
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
    }
}