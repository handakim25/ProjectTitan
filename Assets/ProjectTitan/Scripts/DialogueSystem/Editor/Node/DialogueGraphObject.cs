using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.DialogueSystem.Data
{
    /// <summary>
    /// Dialogue Graph를 저장하는 Object.
    /// </summary>
    public class DialogueGraphObject : ScriptableObject
    {
        public string GraphName;

        public void Init(string graphName)
        {
            GraphName = graphName;
        }
    }
}
