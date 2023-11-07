using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    /// <summary>
    /// Dialogue Graph Object로부터 생성되는 Dialogue Object. 게임 내에서 실질적으로 사용된다.
    /// </summary>
    public class DialogueObject : ScriptableObject, IRefereceable
    {
        public string DialogueName;
        public string StartingNodeID;
        public List<DialogueNode> Nodes = new();

        private Dictionary<string, DialogueNode> _nodeDictionary = new();

        public string ID => DialogueName;

        public DialogueNode GetNode(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return null;
            }
            else if(!_nodeDictionary.ContainsKey(id))
            {
                Debug.LogError($"Dialogue Object : {DialogueName} does not have node : {id}");
                return null;
            }
            return _nodeDictionary[id];
        }

        public DialogueNode GetStartingNode()
        {
            return GetNode(StartingNodeID);
        }

        /// <summary>
        /// 사용될 때 Dictionary 초기화. 실질적으로는 Dictinoary를 이용한다.
        /// </summary>
        private void OnEnable()
        {
            Debug.Log($"Dialogue : {DialogueName} has been enabled.");
            _nodeDictionary.Clear();
            foreach (DialogueNode node in Nodes)
            {
                _nodeDictionary.Add(node.NodeID, node);
            }
        }

        
    }
}
