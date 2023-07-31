using UnityEngine;
namespace Titan
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "ScriptableObjects/FloatVariable")]
    public class FloatVariable : ScriptableObject
    {
        public event System.Action<FloatVariable> OnValueChange;
    
        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChange?.Invoke(this);
            }
        }
    }
}