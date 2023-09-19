using System.Collections;
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

    // Think
    // Graph Object Data가 굳이 View를 알아야 할까?

    /// <summary>
    /// Dialogue Graph를 저장하는 Object.
    /// 에디팅은 <see cref="DialogueEditorWindow"/>에서 이루어진다.
    /// 현재는 Save 버튼을 누르면 Snap Shot을 찍듯이 저장한다.
    /// </summary>
    public class DialogueGraphObject : ScriptableObject
    {
        [Serializable]
        public struct DialogueNodeData
        {
            public string Type;
            public string SerializeData;
        }

        public List<DialogueNodeData> _serializedNodes = new();

        const string kDialogueObjectPath = "Assets/ProjectTitan/ResourcesData/Resources/DataSO/DialogueSO";
        [SerializeField] private string _DialogueSOGUID = string.Empty;

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
        public void UpdateDialogueObject(DialogueGraphView graph)
        {
            var dialogueSo = GetDialogueObject();
            dialogueSo.DialogueName = name;

            var builder = new DialogueBuilder(dialogueSo, graph);
            builder.UpdateDialogueObject();

            // Save Dialogue Object
            EditorUtility.SetDirty(dialogueSo);
            AssetDatabase.SaveAssetIfDirty(dialogueSo);

            var path = AssetDatabase.GUIDToAssetPath(_DialogueSOGUID);
            if(path != $"{kDialogueObjectPath}/{name}.asset")
            {
                AssetDatabase.MoveAsset(path, $"{kDialogueObjectPath}/{name}.asset");
            }
            AssetDatabase.Refresh();
        }

        // private string GetConnectedNode(Port)

        // Load DialogueObject from Guid
        // if file is not exist, create new one
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

        public void LoadData(DialogueGraphView graph)
        {
            var nodeDic = new Dictionary<string, DialogueBaseNodeView>();
            foreach(var serializeNode in _serializedNodes)
            {
                Type type = Type.GetType(serializeNode.Type);
                var nodeView = Activator.CreateInstance(type) as DialogueBaseNodeView;
                JsonUtility.FromJsonOverwrite(serializeNode.SerializeData, nodeView);
                nodeView.Initialize(graph, nodeView.ID);

                graph.AddElement(nodeView);
                nodeDic.Add(nodeView.ID, nodeView);
            }

            var portDic = new Dictionary<string, Port>();
            foreach(var port in graph.ports)
            {
                var portData = port.userData as PortData;
                portDic.Add(portData.PortID, port);
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
