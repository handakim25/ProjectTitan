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
        /// <summary>
        /// Dialgoue 데이터의 이름
        /// </summary>
        public string DialogueName;
        public string StartingNodeID;
        public List<DialogueNode> Nodes = new();

        /// <summary>
        /// Key : NodeID, Value : Node
        /// </summary>
        // @Note
        // private로 되어 있으므로 SO 상태에서는 직렬화되어 있지 않다.
        // 만약 추후에 Json으로 변경할 경우 수정 필요
        private Dictionary<string, DialogueNode> _nodeDictionary = new();

        // IReferenceable
        public string ID => DialogueName;

        /// <summary>
        /// NodeID를 이용해서 Node를 가져온다.
        /// </summary>
        /// <param name="id">Node ID</param>
        /// <returns>찾지 못할 경우 null </returns>
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
        /// 사용될 때 Dictionary 초기화. 실질적으로는 Dictinoary를 이용한다. 만약 JSon으로 대체할 경우 수정할 필요가 있다.
        /// </summary>
        private void OnEnable()
        {
            _nodeDictionary.Clear();
            foreach (DialogueNode node in Nodes)
            {
                _nodeDictionary.Add(node.NodeID, node);
            }
        }

        
    }
}
