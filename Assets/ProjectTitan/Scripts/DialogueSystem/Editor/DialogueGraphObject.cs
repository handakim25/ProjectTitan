using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Titan.DialogueSystem.Data.View;
using Titan.DialogueSystem.Data.Nodes;
using System;
using System.Linq;
using static Titan.DialogueSystem.Data.Nodes.DialogueBaseNodeView;

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

        // public List<DialogueNodeData> Nodes = new List<DialogueNodeData>();
        // public List<DialogueBaseNodeConnectionData> Connections = new List<DialogueBaseNodeConnectionData>();
        public List<DialogueNodeData> _serializedNodes = new();

        public string GraphName;

        public void Init(string graphName)
        {
            GraphName = graphName;
        }

        public void SaveData(DialogueGraphView graph)
        {
            _serializedNodes.Clear();
            
            foreach(var node in graph.nodes)
            {
                var nodeData = new DialogueNodeData() { Type = node.GetType().FullName, SerializeData = JsonUtility.ToJson(node as DialogueBaseNodeView) };
                Debug.Log($"node type : {nodeData.Type}");
                Debug.Log($"serialied node : {nodeData.SerializeData}");
                _serializedNodes.Add(nodeData);
            }
        }

        public void LoadData(DialogueGraphView graph)
        {
            var nodeDic = new Dictionary<string, DialogueBaseNodeView>();
            foreach(var serializeNode in _serializedNodes)
            {
                Debug.Log($"node type : {serializeNode.Type}");
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

            Debug.Log($"port count : {portDic.Count} ");

            foreach(var port in graph.ports)
            {
                var portData = port.userData as PortData;
                Debug.Log($"port data / id : {portData.PortID} / connected id : {portData.ConnectedPortID}");
                if(!string.IsNullOrEmpty(portData.ConnectedPortID))
                {
                    var targetPort = portDic[portData.ConnectedPortID];
                    var edge = port.ConnectTo(targetPort);
                    graph.AddElement(edge);
                }
            }
        }
    }
}
