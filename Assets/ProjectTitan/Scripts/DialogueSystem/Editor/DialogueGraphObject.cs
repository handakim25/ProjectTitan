using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Titan.DialogueSystem.Data.View;
using Titan.DialogueSystem.Data.Nodes;
using System;
using System.Linq;
using static Titan.DialogueSystem.Data.Nodes.DialogueBaseNodeView;
using UnityEditor;

namespace Titan.DialogueSystem.Data
{
    // @refactor
    // node data -> graph object -> graph view
    // node update -> graph object update
    // just save

    // @Memo
    // Graph Object Data가 굳이 View를 알아야 할까?

    /// <summary>
    /// Dialogue Graph를 저장하는 Object.
    /// 에디팅은 <see cref="DialogueEditorWindow"/>에서 이루어진다.
    /// 현재는 Save 버튼을 누르면 Snap Shot을 찍듯이 저장한다.
    /// </summary>
    public class DialogueGraphObject : ScriptableObject
    {
        /// <summary>
        /// Node를 직렬화해서 저장하기 위한 구조체
        /// </summary>
        [Serializable]
        public struct DialogueNodeData
        {
            public string Type;
            public string SerializeData;
        }

        public List<DialogueNodeData> _serializedNodes = new();

        /// <summary>
        /// Dialgoue Object를 저장할 경로. Graph의 결과물이다.
        /// </summary>
        const string kDialogueObjectPath = "Assets/ProjectTitan/ResourcesData/Resources/DataSO/DialogueSO";
        // @refactor
        // readonly로 변경할 것
        /// <summary>
        /// DialogueObject의 GUID, 이를 기반으로 Dialogue Object를 Update한다.
        /// </summary>
        [SerializeField] private string _DialogueSOGUID = string.Empty;

        /// <summary>
        /// Graph의 정보를 직렬화 한다. 저장하기 위해 Snapshot을 저장.
        /// </summary>
        /// <param name="graph"></param>
        public void SaveData(DialogueGraphView graph)
        {
            _serializedNodes.Clear();
            
            foreach(var node in graph.nodes)
            {
                var nodeData = new DialogueNodeData() { Type = node.GetType().FullName, SerializeData = JsonUtility.ToJson(node as DialogueBaseNodeView) };
                _serializedNodes.Add(nodeData);
            }
        }

        // @refactor
        // DialogueGraphObject가 하는 일이 너무 많아졌다.
        // 일단은 여기에 작성하고 분할을 해야 한다.
        /// <summary>
        /// Graph에 해당하는 Dialgoue Object를 업데이트한다.
        /// </summary>
        /// <param name="graph"></param>
        public void UpdateDialogueObject(DialogueGraphView graph)
        {
            var dialogueSo = GetDialogueObject();
            dialogueSo.DialogueName = name;

            var builder = new DialogueBuilder(dialogueSo, graph);
            builder.Build();

            // Save Dialogue Object
            EditorUtility.SetDirty(dialogueSo);
            AssetDatabase.SaveAssetIfDirty(dialogueSo);

            var path = AssetDatabase.GUIDToAssetPath(_DialogueSOGUID);
            // 만약 Graph 이름이 변경되었다면 Dialogue Object의 이름도 변경
            if(path != $"{kDialogueObjectPath}/{name}.asset")
            {
                AssetDatabase.MoveAsset(path, $"{kDialogueObjectPath}/{name}.asset");
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// GUID로부터 Dialogue Object를 로드한다. 만약 없을 경우 새로 생성한다.
        /// </summary>
        /// <returns></returns>
        public DialogueObject GetDialogueObject()
        {
            var path = AssetDatabase.GUIDToAssetPath(_DialogueSOGUID);
            Debug.Log($"Path : {path}");

            DialogueObject dialogueObject = AssetDatabase.LoadAssetAtPath<DialogueObject>(path);
            if(dialogueObject == null)
            {
                dialogueObject = CreateInstance<DialogueObject>();
                AssetDatabase.CreateAsset(dialogueObject, $"{kDialogueObjectPath}/{name}.asset");
                _DialogueSOGUID = AssetDatabase.AssetPathToGUID($"{kDialogueObjectPath}/{name}.asset");

                Debug.Log($"{kDialogueObjectPath}/{name}.asset Created");
            }

            return dialogueObject;
        }

        /// <summary>
        /// 직렬화되어 있는 데이터를 이용해서 Graph를 로드한다.
        /// </summary>
        /// <param name="graph">로드할 Graph</param>
        public void LoadData(DialogueGraphView graph)
        {
            var nodeDic = new Dictionary<string, DialogueBaseNodeView>();
            // Load Nodes
            foreach(var serializeNode in _serializedNodes)
            {
                Type type = Type.GetType(serializeNode.Type);
                var nodeView = Activator.CreateInstance(type) as DialogueBaseNodeView;
                JsonUtility.FromJsonOverwrite(serializeNode.SerializeData, nodeView);
                nodeView.Initialize(graph, nodeView.ID);

                graph.AddElement(nodeView);
                nodeDic.Add(nodeView.ID, nodeView);
            }

            // Make Port Dictionary
            var portDic = new Dictionary<string, Port>();
            var missingPort = new List<PortData>();
            foreach(var port in graph.ports)
            {
                var portData = port.userData as PortData;
                if(string.IsNullOrEmpty(portData.PortID))
                {
                    Debug.LogError("Port ID is null");
                    Debug.Log($"Port : {port}");
                    Debug.Log($"Port Data : {portData}");
                    missingPort.Add(portData);
                }
                else
                {
                    portDic.Add(portData.PortID, port);
                }
            }
            foreach(var portData in missingPort)
            {

            }

            foreach(var port in graph.ports)
            {
                var portData = port.userData as PortData;
                if(!string.IsNullOrEmpty(portData.ConnectedPortID))
                {
                    if(portDic.TryGetValue(portData.ConnectedPortID, out var targetPort))
                    {
                        var edge = port.ConnectTo(targetPort);
                        graph.AddElement(edge);
                    }
                    else
                    {
                        Debug.LogError($"Port Not Found : {portData.ConnectedPortID}");
                    }
                }
            }
        }
    }
}
