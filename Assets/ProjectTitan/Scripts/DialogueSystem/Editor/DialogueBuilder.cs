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
    /// <summary>
    /// Dialogue Graph를 이용해서 Dialogue Object를 생성하는 클래스
    /// Dialouge Object는 DialogueNode의 리스트이다.
    /// </summary>
    public class DialogueBuilder
    {
        public DialogueObject DialogueObject;
        private DialogueGraphView _graph;
        // Key : ID, Value : Node
        private Dictionary<string, DialogueNode> _nodeResult = new();

        // Cache
        private Dictionary<string, DialogueBaseNodeView> _nodeViewMap = new();
        private Dictionary<string, PortData> _portDataMap = new();
        public DialogueBuilder(DialogueObject dialogueObject, DialogueGraphView graph)
        {
            DialogueObject = dialogueObject;
            _graph = graph;

            // Cache Node Views
            foreach(var nodeView in graph.nodes)
            {
                if(nodeView is DialogueBaseNodeView dialogueNodeView)
                {
                    _nodeViewMap.Add(dialogueNodeView.ID, dialogueNodeView);
                }
            }

            // Cache Port Data
            foreach(var port in graph.ports)
            {
                var portData = port.userData as PortData;
                _portDataMap.Add(portData.PortID, portData);
            }
        }

        public void UpdateDialogueObject()
        {
            ProcessSentences();

            ProcessSelectors();

            DialogueObject.Nodes = _nodeResult.Values.ToList();
        }

        private void ProcessSentences()
        {
            foreach (var sentenceNodeView in _graph.nodes.OfType<DialogueSentenceNodeView>())
            {
                DialogueNode dialogueNode = CraeteDialogueNodeFromView(sentenceNodeView);
                if (string.IsNullOrEmpty(sentenceNodeView.inputPortData.ConnectedPortID))
                {
                    DialogueObject.StartingNodeID = dialogueNode.NodeID;
                }
                _nodeResult.Add(dialogueNode.NodeID, dialogueNode);
            }
        }

        private DialogueNode CraeteDialogueNodeFromView(DialogueSentenceNodeView sentenceNodeView)
        {
            var dialogueNode = new DialogueNode();
            dialogueNode.NodeID = sentenceNodeView.ID;
            dialogueNode.SpeakerName = "test";
            dialogueNode.DialogueText = sentenceNodeView.Sentence;
            dialogueNode.TriggerEventID = "test";
            dialogueNode.TriggerSetValue = false;
            dialogueNode.TriggerQuest = sentenceNodeView.TriggerQuestID;
            dialogueNode.TriggerQuestState = sentenceNodeView.QuestStatus.ToString();
            var connectedNode = FindNodeFromPortID(sentenceNodeView.outputPortData.ConnectedPortID);
            dialogueNode.NextNode = connectedNode?.ID ?? null;
            dialogueNode.Choices = new List<Choice>();

            return dialogueNode;
        }

        private void ProcessSelectors()
        {
            foreach (var selectorNodeView in _graph.nodes.OfType<DialogueSelectorNodeView>())
            {
                var connectedSentenceView = FindNodeFromPortID(selectorNodeView.dialogueInputPortData.ConnectedPortID);
                if (connectedSentenceView == null)
                {
                    continue;
                }
                var dialogueNode = _nodeResult[connectedSentenceView.ID];
                dialogueNode.NextNode = null;

                foreach (var pair in selectorNodeView.selectionPortData)
                {
                    ProcessSelectorChoice(pair, dialogueNode.Choices);
                }
            }
        }

        private void ProcessSelectorChoice(PortDataPair portDataPair, List<Choice> choices)
        {
            if(string.IsNullOrEmpty(portDataPair.inputPortData.ConnectedPortID))
            {
                return;
            }

            var connectedNode = FindNodeFromPortID(portDataPair.inputPortData.ConnectedPortID);
            if(connectedNode == null)
            {
                return;
            }

            var choice = new Choice();
            if(connectedNode is DialougeLogicNodeView logicNodeView)
            {
                choice.Condition = logicNodeView.GetCondtion();
                var conditionPortData = logicNodeView.GetConditionPortsData();
                foreach(var conditionPort in conditionPortData)
                {
                    var connectedConditionNode = FindNodeFromPortID(conditionPort.ConnectedPortID);
                    if(connectedConditionNode is DialogueConditionNodeView conditionNodeView)
                    {
                        choice.Condition.Requirements.Add(conditionNodeView.GetRequirement());
                    }
                }

                // proceed to next node
                connectedNode = FindNodeFromPortID(logicNodeView._choiceInputPortData.ConnectedPortID);
            }
            // 현재는 중첩된 Logic Node를 지원하지 않는다.
            if(connectedNode is DialogueChoiceNodeView choiceNodeView)
            {
                choice.ChoiceText = choiceNodeView.Sentence;
                choice.NextNode = FindNodeFromPortID(portDataPair.outputPortData.ConnectedPortID)?.ID ?? null;
                choices.Add(choice);
            }
        }

        #region Utility

        private DialogueBaseNodeView FindNodeFromPortData(PortData portData)
        {
            if(portData == null)
            {
                return null;
            }

            if (!_portDataMap.TryGetValue(portData.PortID, out var port))
            {
                Debug.LogError($"PortData is not found. PortID : {portData.PortID}");
                return null;
            }
            if (!_nodeViewMap.TryGetValue(port.nodeId, out var nodeView))
            {
                Debug.LogError($"NodeView is not found. NodeID : {port.nodeId}");
                return null;
            }

            return nodeView;
        }

        /// <summary>
        /// Port ID로 NodeView를 찾는다.
        /// </summary>
        /// <param name="portID">string port ID</param>
        /// <returns>NodeView, 만약에 없을 경우 null 반환</returns>
        private DialogueBaseNodeView FindNodeFromPortID(string portID)
        {
            if(string.IsNullOrEmpty(portID))
            {
                return null;
            }
            if(!_portDataMap.TryGetValue(portID, out var portData))
            {
                Debug.LogError($"PortData is not found. PortID : {portID}");
                return null;
            }

            return FindNodeFromPortData(portData);
        }
        
        #endregion Utility
    }
}
