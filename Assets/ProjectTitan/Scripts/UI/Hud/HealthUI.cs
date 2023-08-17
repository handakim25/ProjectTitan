using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Titan
{
    // To-Do
    // 지연된 Health UI 그리기 위해 분리한 Component
    [RequireComponent(typeof(Slider))]
    public class HealthUI : MonoBehaviour
    {
        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void UpdateHealthUI(float maxHealth, float curHealth)
        {
            float curPerecntage = curHealth / maxHealth;
            _slider.value = curPerecntage;
        }
    }
}
