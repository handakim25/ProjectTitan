using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Titan.DialogueSystem.Data.View;
using Titan.DialogueSystem.Data.Nodes;
using System;
using System.Linq;

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
        public struct DialogueNodeData
        {
            public Type type;
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
                var nodeData = new DialogueNodeData() { type = node.GetType(), SerializeData = JsonUtility.ToJson(node as DialogueBaseNodeView) };
                _serializedNodes.Add(nodeData);
            }
        }

        public void LoadData(DialogueGraphView graph)
        {
            var nodeDic = new Dictionary<string, DialogueBaseNodeView>();
            foreach(var serializeNode in _serializedNodes)
            {
                var nodeView = Activator.CreateInstance(serializeNode.type) as DialogueBaseNodeView;
                JsonUtility.FromJsonOverwrite(serializeNode.SerializeData, nodeView);
                nodeView.Initialize(graph, nodeView.ID);

                graph.AddElement(nodeView);
                Debug.Log($"node id : {nodeView.ID}");
                nodeDic.Add(nodeView.ID, nodeView);
            }

            foreach(var nodeView in nodeDic.Values)
            {
                var outputPorts = nodeView.outputContainer.Children().Cast<Port>().ToList();
                for(int i = 0; i < nodeView.OutputPortIds.Count; i++)
                {
                    string targetID = nodeView.OutputPortIds[i];
                    Debug.Log($"Target ID : {targetID}");
                    var targetNode = nodeDic[targetID];
                    var outputPort = outputPorts[i];

                    Edge edge = outputPort.ConnectTo(targetNode.inputContainer[0] as Port);
                    graph.AddElement(edge);
                    Debug.Log($"Edge Connect");
                }
            }
        }
    }
}
