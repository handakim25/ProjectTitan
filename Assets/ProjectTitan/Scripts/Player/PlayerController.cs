using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.UI;

namespace Titan.Character
{
    /// <summary>
    /// Player의 데이터를 가지고 처리한다.
    /// </summary>
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public bool IsWalk;

        public event System.Action<PlayerController> PlayerDataChanged;

        private void LateUpdate()
        {
            
        }
    }
}
