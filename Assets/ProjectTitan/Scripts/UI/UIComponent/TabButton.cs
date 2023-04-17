using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using DG.Tweening;

namespace Titan.UI
{
    public static class ColorExtentions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g,color.b, alpha);
        }
    }

    [RequireComponent(typeof(Image))]   
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public TabGroup tabGroup;

        [SerializeField] private Image _background;
        [SerializeField] private Image _targetImage; // image in child

        [field : SerializeField] public float TransitionTime {get; protected set;} = 1.0f;

        [SerializeField] Color _normalColor;
        [SerializeField] Color _hightlightedColor;
        [SerializeField] Color _pressedColor;
        [SerializeField] Color _selectedColor;

        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeslected;

        bool _isClicked = false;
        bool _isSelected = false;

        private void Start()
        {
            _background = GetComponent<Image>();
            tabGroup?.Subscribe(this);
        }

        private void OnEnable()
        {
            _isClicked = false;
            if(_targetImage)
            {
                _targetImage.color = _normalColor;
            }
        }

        #region EventSystem Callback
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(_isSelected)
                return;
            
            Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_isSelected)
                return;

            // 만약 PointerExit 등의 Tween이 있으면 취소
            DOTween.Kill(transform);

            // Button is not clicked yet.
            // Hover image
            if(_targetImage && !_isClicked)
            {
                Sequence enterSequence = DOTween.Sequence();
                enterSequence.Append(_targetImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), TransitionTime));
                enterSequence.Join(_targetImage.DOColor(_hightlightedColor, TransitionTime));
                enterSequence.SetTarget(transform);
                Debug.Log($"To hilight color");
            }
            // Button was clicked before enter.
            // Go to clicked state
            else if(_targetImage && _isClicked)
            {
                Sequence enterClickSequence = DOTween.Sequence();
                enterClickSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                enterClickSequence.Join(_targetImage.DOColor(_pressedColor, TransitionTime));
                enterClickSequence.SetTarget(transform);
                Debug.Log($"To pressed color");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Tab Pointer Exit");
            if(_isSelected)
                return;

            // Go to normal state
            if(_targetImage)
            {
                Sequence exitSequence = DOTween.Sequence();
                exitSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                exitSequence.Join(_targetImage.DOColor(_normalColor, 1.0f));
                exitSequence.SetTarget(transform);
                Debug.Log($"To normal color");
            }
        }

        // Change to pressed button
        public void OnPointerDown(PointerEventData eventData)
        {
            if(_isSelected)
                return;

            _isClicked = true;

            if(_targetImage)
            {
                Sequence pressedSequence = DOTween.Sequence();
                pressedSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                pressedSequence.Join(_targetImage.DOColor(_pressedColor, 1.0f));
                pressedSequence.SetTarget(transform);
                Debug.Log($"Pressed target : {pressedSequence.target}");
                
                Debug.Log($"To pressed color");
            }
        }
        
        #endregion EventSystem Callback        

        #region Methods
        
        // Select can be called from outside
        public void Select()
        {
            if(_isSelected)
            {
                return;
            }

            _isSelected = true;

            if(_targetImage)
            {
                DOTween.Kill(transform);
                _targetImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                _targetImage.color = _selectedColor;
            }

            if(_background)
            {
                _background.color = _background.color.WithAlpha(1.0f);
            }

            OnTabSelected?.Invoke();
            tabGroup?.OnTabSelected(this);
        }

        public void Deselect()
        {
            _isSelected = false;
            _isClicked = false;

            if(_targetImage)
            {
                int killTransform = DOTween.Kill(transform);
                _targetImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                _targetImage.color = _normalColor;
            }

            if(_background)
            {
                _background.color = _background.color.WithAlpha(0.0f);
            }
            
            OnTabDeslected?.Invoke();
            tabGroup?.OnTabDeslected(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isClicked = false;
        }

        #endregion Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_targetImage)
            {
                _targetImage.color = _normalColor;
            }
        }
#endif
    }
}
