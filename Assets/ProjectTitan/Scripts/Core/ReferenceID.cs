using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    [System.Serializable]
    public class ReferenceID<T> where T : IRefereceable
    {
        public string ID;

#if UNITY_EDITOR
        [SerializeField] private T reference;
#endif
    }
}
