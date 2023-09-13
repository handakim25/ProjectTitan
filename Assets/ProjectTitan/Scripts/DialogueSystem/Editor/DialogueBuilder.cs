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
            // Selector Node의 선택지 리스트
            public List<PortDataPair> PortData;
        }
        public DialogueObject DialogueObject;
        private DialogueGraphView _graph;

        // look up
        /// <summary>
        /// Key : Node ID, Value : NodeView
        /// </summary>
        private Dictionary<string, DialogueBaseNodeView> _nodeViewDic = new();
        /// <summary>
        /// key : Node ID, value : DialogueNode
        /// </summary>
        private Dictionary<string, DialogueNode> _dialogueNodeDic = new();
        /// <summary>
        /// key : Port ID, value : PortData
        /// </summary>
        private Dictionary<string, PortData> _portDataDic = new();

        // List to process
        private List<DialogueChoiceNodeView> _choiceNodeViews = new();
        private List<DialogueSentenceNodeView> _sentenceNodeViews = new();
        private Dictionary<DialogueSelectorNodeView, SelectorData> _selectorDataDic = new();

        public DialogueBuilder(DialogueObject dialogueObject, DialogueGraphView graph)
        {
            DialogueObject = dialogueObject;
            _graph = graph;

            // create look up table
            foreach(var node in _graph.nodes)
            {
                var nodeView = node as DialogueBaseNodeView;
                _nodeViewDic.Add(nodeView.ID, nodeView);

                if(nodeView is DialogueSentenceNodeView sentenceNodeView)
                {
                    var dialogueNode = new DialogueNode
                    {
                        NodeID = sentenceNodeView.ID,
                        SpeakerName = "Test Speaker",
                        DialogueText = sentenceNodeView.Sentence,
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

            DialogueObject.Nodes = _dialogueNodeDic.Values.ToList();
        }

        // Choice Data를 기준으로 Selector Node에 Choice 정보를 생성한다.
        // Choice Node는 Logic Node 혹은 Selector Node에 연결될 수 있다.
        // 목적은 Choice Data를 생성하는 것이다.
        private void ProcessChoiceData()
        {
            foreach(var choiceNodeView in _choiceNodeViews)
            {
                var nextNode = GetConnectedNode(choiceNodeView.choicePortData);
                if(nextNode == null)
                {
                    continue;
                }
                else if(nextNode is DialougeLogicNodeView logicNodeView && logicNodeView.IsValid())
                {
                    
                }
            }
        }

        /// <summary>
        /// Dialogue Node들을 연결한다.
        /// Selector Node View는 연결되어 있는 Dialogue Node에 연결이 된다.
        /// </summary>
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
            else
            {
                var connectedPort = _portDataDic[portData.ConnectedPortID];
                return connectedPort.NodeView;
            }
        }
    }
}
