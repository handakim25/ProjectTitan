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
        // public List<DialogueNodeData> Nodes = new List<DialogueNodeData>();
        // public List<DialogueBaseNodeConnectionData> Connections = new List<DialogueBaseNodeConnectionData>();
        public List<string> _serializedNodes = new();
        public List<Type> _nodeTypes = new();

        public string GraphName;

        public void Init(string graphName)
        {
            GraphName = graphName;
        }

        public void SaveData(DialogueGraphView graph)
        {
            _serializedNodes.Clear();
            _nodeTypes.Clear();
            
            foreach(var node in graph.nodes)
            {
                _serializedNodes.Add(JsonUtility.ToJson(node as DialogueBaseNodeView));
                Debug.Log($"Node Type : {node.GetType()}");
                _nodeTypes.Add(node.GetType());
            }
        }

        public void LoadData(DialogueGraphView graph)
        {
            var nodes = new Dictionary<string, DialogueBaseNodeView>();
            for (int i = 0; i < _serializedNodes.Count; i++)
            {
                string node = _serializedNodes[i];
                Type type = _nodeTypes[i];
                var nodeView = Activator.CreateInstance(type) as DialogueBaseNodeView;
                JsonUtility.FromJsonOverwrite(node, nodeView);
                nodeView.Initialize(graph, nodeView.ID);
                graph.AddElement(nodeView);
                Debug.Log($"node id : {nodeView.ID}");
                nodes.Add(nodeView.ID, nodeView);
            }

            Debug.Log("Dictinoary Created");
            foreach((string key, DialogueBaseNodeView view) in nodes)
            {
                Debug.Log($"Key : {key}, Value : {view}");
            }


            foreach(var nodeView in nodes.Values)
            {
                var outputPorts = nodeView.outputContainer.Children().Cast<Port>().ToList();
                for(int i = 0; i < nodeView.OutputPortIds.Count; i++)
                {
                    string targetID = nodeView.OutputPortIds[i];
                    Debug.Log($"Target ID : {targetID}");
                    var targetNode = nodes[targetID];
                    var outputPort = outputPorts[i];

                    Edge edge = outputPort.ConnectTo(targetNode.inputContainer[0] as Port);
                    graph.AddElement(edge);
                    Debug.Log($"Edge Connect");
                }
            }
        }
    }
}
