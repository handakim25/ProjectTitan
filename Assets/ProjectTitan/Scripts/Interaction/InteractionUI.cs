using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Interaction;

namespace Titan.UI.Interaction
{
    /// <summary>
    /// Interaction Slot UI Controller
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        /// <summary>
        /// InteractonUI가 가리키고 있는 Interactable Object
        /// GameObject로 가리키고 있는 것은 Interact 가능 오브젝트들이 입력으로 오기 때문
        /// </summary>
        public GameObject Interactable {get; set;}

        public void SelectSlot()
        {

        }

        public void DeSelectSlot()
        {

        }
    }
}
