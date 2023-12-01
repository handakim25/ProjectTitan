using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Titan.DialogueSystem.Data.Nodes;
using Titan.DialogueSystem.Data.View;
using static Titan.DialogueSystem.Data.Nodes.DialogueBaseNodeView;

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
        /// <summary>
        /// Key : NodeID, Value : DialogueNode
        /// Build를 한 결과물
        /// </summary>
        private Dictionary<string, DialogueNode> _nodeResult = new();

        /// <summary>
        /// Key : NodeID, Value : NodeView
        /// </summary>
        private Dictionary<string, DialogueBaseNodeView> _nodeViewMap = new();
        /// <summary>
        /// Key : PortID, Value : PortData
        /// </summary>
        private Dictionary<string, PortData> _portDataMap = new();

        public DialogueBuilder(DialogueObject dialogueObject, DialogueGraphView graph)
        {
            DialogueObject = dialogueObject;
            _graph = graph;
            if(dialogueObject == null || graph == null)
            {
                Debug.LogError("DialogueObject or DialogueGraphView is null");
                return;
            }

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

        /// <summary>
        /// Dialogue Grpah를 이용해서 Dialogue Object를 업데이트한다.
        /// </summary>
        public void Build()
        {
            ProcessSentences();

            ProcessSelectors();

            DialogueObject.Nodes = _nodeResult.Values.ToList();
        }

        /// <summary>
        /// Sentence Node를 처리한다. _nodeResult에 추가한다.
        /// </summary>
        private void ProcessSentences()
        {
            foreach (var sentenceNodeView in _graph.nodes.OfType<DialogueSentenceNodeView>())
            {
                DialogueNode dialogueNode = CraeteDialogueNodeFromView(sentenceNodeView);
                // Input Port가 연결되어 있지 않으면 Starting Node로 설정한다.
                if (string.IsNullOrEmpty(sentenceNodeView.inputPortData.ConnectedPortID))
                {
                    DialogueObject.StartingNodeID = dialogueNode.NodeID;
                }
                _nodeResult.Add(dialogueNode.NodeID, dialogueNode);
            }
        }

        // @Refactor
        // Node View와 Node Data를 동일하게 처리하면 실수를 줄이고 간결하게 작성할 수 있을 것 같다.
        private DialogueNode CraeteDialogueNodeFromView(DialogueSentenceNodeView sentenceNodeView)
        {
            var dialogueNode = new DialogueNode();
            dialogueNode.NodeID = sentenceNodeView.ID;
            dialogueNode.SpeakerName = "";
            dialogueNode.SentenceText = sentenceNodeView.Sentence;
            dialogueNode.TriggerEventID = "";
            dialogueNode.TriggerSetValue = false;
            dialogueNode.TriggerQuest = sentenceNodeView.TriggerQuestID;
            dialogueNode.TriggerQuestState = sentenceNodeView.QuestStatus.ToString();
            var connectedNode = FindNodeFromPortID(sentenceNodeView.outputPortData.ConnectedPortID);
            dialogueNode.NextNode = connectedNode?.ID ?? null;
            dialogueNode.Choices = new List<Choice>();

            return dialogueNode;
        }

        /// <summary>
        /// Selector Node들을 처리한다. Selector Node를 기준으로 연결된 Sentence Node에 선택지 정보를 추가한다.
        /// </summary>
        private void ProcessSelectors()
        {
            foreach (var selectorNodeView in _graph.nodes.OfType<DialogueSelectorNodeView>())
            {
                // Sentence Node는 Dialogue Node에서 넘어왔기 때문에 이전 노드를 찾으면 연결된 노드를 찾을 수 있다.
                // 만약에 연결된 노드가 없으면 의미 없는 노드이다.
                // nextNode의 경우 의미가 없ㅇ므으로 null로 설정한다.
                var connectedFromSentenceView = FindNodeFromPortID(selectorNodeView.dialogueInputPortData.ConnectedPortID);
                if (connectedFromSentenceView == null)
                {
                    continue;
                }
                var dialogueNode = _nodeResult[connectedFromSentenceView.ID];
                dialogueNode.NextNode = null;

                foreach (var pair in selectorNodeView.selectionPortData)
                {
                    ProcessSelectorChoice(pair, dialogueNode.Choices);
                }
            }
        }

        /// <summary>
        /// Port Data를 기준으로 대사 노드에 선택지를 추가한다.
        /// </summary>
        /// <param name="portDataPair"></param>
        /// <param name="choices"></param>
        private void ProcessSelectorChoice(PortDataPair portDataPair, List<Choice> choices)
        {
            if(string.IsNullOrEmpty(portDataPair.inputPortData.ConnectedPortID))
            {
                return;
            }

            var connectedFromNode = FindNodeFromPortID(portDataPair.inputPortData.ConnectedPortID);
            if(connectedFromNode == null)
            {
                return;
            }

            // Selector Node에 연결된 노드는 LogiceNodeView 혹은 ChoiceNodeView이다.
            // ChoiceNodeView는 그대로 Choice에 연결하면 되고 LogicNodeView는 조건을 추가해야 한다.
            // 따라서 Logic Node View를 처리할 경우, ConnectedFromNode를 연결된 노드로 이동한다.
            var choice = new Choice();
            if(connectedFromNode is DialougeLogicNodeView logicNodeView)
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
                connectedFromNode = FindNodeFromPortID(logicNodeView.ChoiceInputPortData.ConnectedPortID);
            }
            // @Warning
            // 1. 현재는 중첩된 Logic Node를 지원하지 않는다.
            // 2. Logic Node를 처리할 경우 ConnectedFromNode를 이동시키기 때문에 else if 로 하면 Logic Node 처리가 불가능하다.
            if(connectedFromNode is DialogueChoiceNodeView choiceNodeView)
            {
                choice.ChoiceText = choiceNodeView.Sentence;
                choice.NextNode = FindNodeFromPortID(portDataPair.outputPortData.ConnectedPortID)?.ID ?? null;
                choices.Add(choice);
            }
        }

        #region Utility

        /// <summary>
        /// PortData로 NodeView를 찾는다.
        /// </summary>
        /// <param name="portData"></param>
        /// <returns>연결된 Node를 반환하나, 잘못된 값일 경우 null을 반환</returns>
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
                Debug.LogError($"NodeView is not found. NodeID : {portData.nodeId}");
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
