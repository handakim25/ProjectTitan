using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem
{
    public class DialogueObject : ScriptableObject
    {
        public string DialogueName;
        public string StartingNodeID;
        public List<DialogueNode> Nodes = new();

        private Dictionary<string, DialogueNode> _nodeDictionary = new();
        
        public DialogueNode GetNode(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return null;
            }
            else if(!_nodeDictionary.ContainsKey(id))
            {
                return null;
            }
            return _nodeDictionary[id];
        }

        public DialogueNode GetStartingNode()
        {
            return GetNode(StartingNodeID);
        }

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
