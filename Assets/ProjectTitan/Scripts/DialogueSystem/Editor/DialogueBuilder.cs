using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.DialogueSystem.Data;
using Titan.DialogueSystem.Data.Nodes;
using UnityEditor.Experimental.GraphView;
using Titan.DialogueSystem.Data.View;
using static Titan.DialogueSystem.Data.Nodes.DialogueBaseNodeView;
using System.Linq;
using System;

namespace Titan.DialogueSystem
{
    public class DialogueBuilder
    {
        private class SelectorData
        {
            public List<Choice> Choices = new();
            public List<PortDataPair> PortData;
        }
        public DialogueObject DialogueObject;
        private DialogueGraphView _graph;

        // key : id, value : BaseNodeView
        private Dictionary<string, DialogueBaseNodeView> _nodeViewDic = new();
        private Dictionary<string, DialogueNode> _dialogueNodeDic = new();
        private List<DialogueChoiceNodeView> _choiceNodeViews = new();
        private List<DialogueSentenceNodeView> _sentenceNodeViews = new();
        private Dictionary<string, PortData> _portDataDic = new();
        private Dictionary<DialogueSelectorNodeView, SelectorData> _selectorDataDic = new();

        public DialogueBuilder(DialogueObject dialogueObject, DialogueGraphView graph)
        {
            DialogueObject = dialogueObject;
            _graph = graph;

            foreach(var node in _graph.nodes)
            {
                var nodeView = node as DialogueBaseNodeView;
                _nodeViewDic.Add(nodeView.ID, nodeView);

                if(nodeView is DialogueSentenceNodeView sentenceNodeView)
                {
                    var dialogueNode = new DialogueNode
                    {
                        NodeID = sentenceNodeView.ID
                    };
                    _dialogueNodeDic.Add(dialogueNode.NodeID, dialogueNode);
                    _sentenceNodeViews.Add(sentenceNodeView);
                }
                else if(nodeView is DialogueChoiceNodeView choiceNodeView)
                {
                    _choiceNodeViews.Add(choiceNodeView);
                }
                else if(nodeView is DialogueSelectorNodeView selectorNodeView)
                {
                    var selectorData = new SelectorData
                    {
                        PortData = selectorNodeView.selectionPortData
                    };
                    _selectorDataDic.Add(selectorNodeView, selectorData);
                }
            }

            foreach(var port in _graph.ports)
            {
                var portData = port.userData as PortData;
                _portDataDic.Add(portData.PortID, portData);
            }
        }

        public void UpdateDialogueObject()
        {
            DialogueObject.Nodes.Clear();

            ProcessChoiceData();

            ConnectNodes();
            // foreach(var node in _graph.nodes)
            // {
            //     if(node is DialogueSentenceNodeView sentenceNodeView)
            //     {
            //         string nodeId = sentenceNodeView.ID;
            //         var dialogueNode = dialogueNodeDic[nodeId];

            //         var outputPortData = sentenceNodeView.outputPortData;
            //         var connectedPort = portDic[outputPortData.ConnectedPortID];
            //         var connectedNode = connectedPort.node;
            //         if(connectedNode is DialogueSentenceNodeView targetSentence)
            //         {
            //             dialogueNode.NextNode = targetSentence.ID;
            //         }
            //         else if(connectedNode is DialogueSelectorNodeView targetSelector)
            //         {
            //             foreach(var portPair in targetSelector.selectionPortData)
            //             {
            //                 var test = portPair.inputPortData.ConnectedPortID;
            //             }
            //         }
            //     }
            //     else if(node is DialogueSelectorNodeView selectorNodeView)
            //     {

            //     }
            // }

            // DialogueObject.Nodes = dialogueNodeDic.Values.ToList();
        }

        private void ProcessChoiceData()
        {
            foreach(var choiceNodeView in _choiceNodeViews)
            {
                var nextNode = GetConnectedNode(choiceNodeView.choicePortData);
                if(nextNode is DialougeLogicNodeView logicNodeView && logicNodeView.IsValid())
                {
                    
                }
            }
        }


        private void ConnectNodes()
        {
            foreach(var sentenceNodeView in _sentenceNodeViews)
            {
                var dialogueNode = _dialogueNodeDic[sentenceNodeView.ID];
                var connectedNodeView = GetConnectedNode(sentenceNodeView.outputPortData);
                if(connectedNodeView is DialogueSentenceNodeView targetSentence)
                {
                    dialogueNode.NextNode = targetSentence.ID;
                }
                else if(connectedNodeView is DialogueSelectorNodeView targetSelector)
                {
                    var SelectorData = _selectorDataDic[targetSelector];
                    foreach(var choice in SelectorData.Choices)
                    {
                        dialogueNode.choices.Add(choice);
                    }
                }
            }
        }

        private DialogueBaseNodeView GetConnectedNode(PortData portData)
        {
            if(string.IsNullOrEmpty(portData.ConnectedPortID))
            {
                return null;
            }

            if(_nodeViewDic.TryGetValue(portData.ConnectedPortID, out var nodeView))
            {
                return nodeView;
            }

            return null;
        }
    }
}
