using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.DialogueSystem.Data.Nodes;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// Node Data를 저장하는 클래스
    /// Shader Graph에서는 Node type을 Slot이라고 저장해서 복잡도를 낮췄지만, 
    /// 일단 현재 구현은 각 Node Type에 대한 Data를 저장하는 클래스를 만들어서 저장하고 있음
    /// </summary>
    [System.Serializable]
    public class DialogueBaseNodeData
    {
        public Vector2 position;
        // ID를 기준으로 연결을 한다.
        public string ID;
        public List<string> outputPortIDs = new();

        public string Text;

        public DialogueBaseNodeView ConcreteNodeView()
        {
            var nodeView = new DialogueSentenceNodeView();
            return nodeView;
        }
    }
}
