using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Character.Player;

namespace Titan.Character
{
    /// <summary>
    /// Player의 데이터를 가지고 처리한다.
    /// </summary>
    [SelectionBase]
    public class PlayerController : MonoBehaviour
    {
        public bool IsWalk;
        /// <summary>
        /// Access From Player Components
        /// </summary>
        public PlayerStatus Status = new();

        public event System.Action<PlayerStatus> PlayerDataChanged;

        public void InitPlayer()
        {
            PlayerDataChanged = null;
        }

        public void ForceUpdateStatus()
        {
            PlayerDataChanged?.Invoke(Status);
            Status.ResetDirty();
        }

        private void LateUpdate()
        {
            if(Status.IsDirty)
            {
                PlayerDataChanged?.Invoke(Status);
                Status.ResetDirty();
            }
        }
    }
}
