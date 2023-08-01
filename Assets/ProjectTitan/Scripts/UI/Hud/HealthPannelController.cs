using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Titan.UI
{
    public class HealthPannelController : MonoBehaviour
    {
        [SerializeField] private FloatVariable _playerHealth;
        [SerializeField] private FloatVariable _playerMaxHealth;

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _healthText;
        bool _hasInit = false;

        private void Awake()
        {
            Debug.Assert(_playerHealth != null);
            Debug.Assert(_playerMaxHealth != null);

            if(_healthSlider == null)
            {
                _healthSlider = transform.Find("Slider").GetComponent<Slider>();
                Debug.Assert(_healthSlider != null);
            }

            if(_healthText == null)
            {
                _healthText = transform.Find("Slider/HealthText").GetComponent<TextMeshProUGUI>();
                Debug.Assert(_healthText != null);
            }
        }

        private void OnEnable()
        {
            _playerHealth.OnValueChange += OnHealthChangeHandler;
            _playerMaxHealth.OnValueChange += OnMaxHealthChangeHandler;

            if(_hasInit)
            {
                UpdateHealthUI();
            }
        }

        private void OnDisable()
        {
            _playerHealth.OnValueChange -= OnHealthChangeHandler;
            _playerMaxHealth.OnValueChange -= OnMaxHealthChangeHandler;
        }

        private void Start()
        {
            _hasInit = true;
            UpdateHealthUI();
        }

        private void OnHealthChangeHandler(FloatVariable variable)
        {
            UpdateHealthUI();
        }

        private void OnMaxHealthChangeHandler(FloatVariable variable)
        {
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            float ratio = _playerHealth.Value / _playerMaxHealth.Value;
            _healthSlider.value = ratio;
            _healthText.text = $"{_playerHealth.Value}/{_playerMaxHealth.Value}";
        }
    }
}
