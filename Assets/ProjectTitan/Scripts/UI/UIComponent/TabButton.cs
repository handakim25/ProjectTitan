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
            // _background = GetComponent<Image>();
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
            }
            // Button was clicked before enter.
            // Go to clicked state
            else if(_targetImage && _isClicked)
            {
                Sequence enterClickSequence = DOTween.Sequence();
                enterClickSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                enterClickSequence.Join(_targetImage.DOColor(_pressedColor, TransitionTime));
                enterClickSequence.SetTarget(transform);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_isSelected)
                return;

            // Go to normal state
            if(_targetImage)
            {
                Sequence exitSequence = DOTween.Sequence();
                exitSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                exitSequence.Join(_targetImage.DOColor(_normalColor, TransitionTime));
                exitSequence.SetTarget(transform);
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
                pressedSequence.Join(_targetImage.DOColor(_pressedColor, TransitionTime));
                pressedSequence.SetTarget(transform);
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
                DOTween.Kill(transform); // DoTween을 Kill하려면 Start 이후에서 호출해야한다.
                _targetImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                _targetImage.color = _selectedColor;
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
