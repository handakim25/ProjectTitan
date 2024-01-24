using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Interaction
{
    // Interaction List를 공유하기 위한 SO
    // Player에서 넣어주고, Interaction View에서 표시해 준다.
    // Player에서 Interaction List를 Update하고 Interaction List에서 Event를 발생시킨다.
    // Event를 발생하면 Interaction view가 Update 된다.
    // Interaction List는 Model이고 Interaction View는 View, Detector는 Controller인가?
    [CreateAssetMenu(fileName = "InteractionList", menuName = "Game Play/InteractionList")]
    public class InteractionList : ScriptableObject
    {
        [SerializeField] private int _maxInteractObjects = 10;
        public int MaxInteractObjects => _maxInteractObjects;
        [System.NonSerialized] public List<GameObject> interactObjects;


        public class InteractSlotChangedEventArgs : System.EventArgs
        {
            public GameObject[] AddedObjects;
            public GameObject[] RemovedObjects;
        }
        public delegate void InteractSlotChangedEventHandler(Object sender, InteractSlotChangedEventArgs handler);
        [System.NonSerialized]
        public InteractSlotChangedEventHandler OnInteractChanged;

        // Scene에 불러왔을 때 List 생성
        private void OnEnable()
        {
            interactObjects = new List<GameObject>();
        }

        public void Clear()
        {
            interactObjects.Clear();
        }

        // Player Detector로부터 입력을 받아서 현재의 Interaction List를 Update
        /// <summary>
        /// Interaction Detector로부터 입력을 받아서 Interaction List를 갱신하고 변화가 있을 경우 이벤트 발생
        /// </summary>
        /// <param name="detectedList">현재 검색한 물체들</param>
        /// <param name="count">검색한 물체의 개수</param>
        public void UpdateInteractionList(GameObject[] detectedList, int count)
        {
            var updateList = detectedList.Take(count);

            var removedObjects = interactObjects.Except(updateList);
            var addedObjects = updateList.Except(interactObjects);

            InteractSlotChangedEventArgs args = null;
            if(removedObjects.Any() || addedObjects.Any())
            {
                args = new InteractSlotChangedEventArgs()
                    {
                        RemovedObjects = removedObjects.Any() ? removedObjects.ToArray() : null,
                        AddedObjects = addedObjects.Any() ? addedObjects.ToArray() : null
                    };
            }

            interactObjects.Clear();
            interactObjects.AddRange(updateList);

            if(args != null)
            {
                OnInteractChanged?.Invoke(this, args);
            }
        }
    }
}