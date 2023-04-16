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
        [SerializeField] private Image _targetImage;

        [field : SerializeField] public float TransitionTime {get; protected set;} = 1.0f;

        [SerializeField] Color _normalColor;
        [SerializeField] Color _hightlightedColor;
        [SerializeField] Color _pressedColor;
        [SerializeField] Color _selectedColor;

        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeslected;

        bool _isClicked = false;
        bool _isSelected = false;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _background = GetComponent<Image>();
            tabGroup?.Subscribe(this);
            var mouse = Mouse.current;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
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
            Debug.Log($"Tab Selected");

            if(_isSelected)
                return;
            
            Select();

            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Tab Pointer enter");

            if(_isSelected)
                return;

            DOTween.Kill(transform);

            if(_targetImage && !_isClicked)
            {
                Sequence enterSequence = DOTween.Sequence();
                enterSequence.Append(_targetImage.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), TransitionTime));
                enterSequence.Join(_targetImage.DOColor(_hightlightedColor, TransitionTime));
                enterSequence.SetTarget(transform);
                Debug.Log($"To hilight color");
            }
            else if(_targetImage && _isClicked)
            {
                Sequence enterClickSequence = DOTween.Sequence();
                enterClickSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                enterClickSequence.Join(_targetImage.DOColor(_pressedColor, TransitionTime));
                enterClickSequence.SetTarget(transform);
                Debug.Log($"To pressed color");
            }

            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Tab Pointer Exit");
            if(_isSelected)
                return;

            if(_targetImage)
            {
                Sequence exitSequence = DOTween.Sequence();
                exitSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime));
                exitSequence.Join(_targetImage.DOColor(_normalColor, 1.0f));
                exitSequence.SetTarget(transform);
                Debug.Log($"To normal color");
            }

            tabGroup.OnTabExit(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(_isSelected)
                return;

            _isClicked = true;

            Debug.Log($"Tab Pointer down");
            _targetImage?.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime); // go to clicked
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
        
        public void Select()
        {
            _isSelected = true;

            if(_targetImage)
            {
                // _targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), TransitionTime);
                // _targetImage.color = _selectedColor;

                // var selectSequence = DOTween.Sequence();
                // selectSequence.Append(_targetImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.1f));
                // selectSequence.Join(_targetImage.DOColor(_selectedColor, 0.0f));
                Debug.Log($"Select.");
                // DOTween.Kill(_targetImage.transform);
                // _targetImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                // DOTween.Kill(_targetImage);
                // _targetImage.color = _selectedColor;
            }

            if(_background)
            {
                var bgColor = _background.color;
                bgColor.a = 1.0f;
                _background.color = bgColor;
            }

            OnTabSelected?.Invoke();
        }

        public void Deselect()
        {
            _isSelected = false;
            _isClicked = false;

            if(_targetImage)
            {
                int killTransform = DOTween.Kill(transform);
                _targetImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Debug.Log($"Killed Transform : {killTransform}");
                _targetImage.color = _normalColor;
            }

            if(_background)
            {
                var bgColor = _background.color;
                bgColor.a = 0.0f;
                _background.color = bgColor;
            }
            
            OnTabDeslected?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isClicked = false;
            Debug.Log($"Tab Pointer Up");
        }


        #endregion Methods
    }
}
