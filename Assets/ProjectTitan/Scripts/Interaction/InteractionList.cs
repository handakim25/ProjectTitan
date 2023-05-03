using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Interaction
{
    [CreateAssetMenu(fileName = "InteractionList", menuName = "InteractionList")]
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

        private void OnEnable()
        {
            interactObjects = new List<GameObject>();
        }

        public void Clear()
        {
            interactObjects.Clear();
        }

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