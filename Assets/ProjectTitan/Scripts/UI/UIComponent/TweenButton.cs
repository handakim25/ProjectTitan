using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using DG.Tweening;

namespace Titan
{
    public class TweenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {

        [SerializeField] protected Image _targetImage; // image in child

        [field : SerializeField] public float TransitionTime {get; protected set;} = 1.0f;

        [SerializeField] protected Color _normalColor = Color.white;
        [SerializeField] protected Color _hightlightedColor = Color.white;
        [SerializeField] protected Color _pressedColor = Color.white;
        [SerializeField] protected Color _selectedColor = Color.white;

        private Vector3 _normalScale;
        [SerializeField] protected Vector3 _highlightScale = new Vector3(1, 1, 1);
        [SerializeField] protected Vector3 _pressedScale = new Vector3(1, 1, 1);
        [SerializeField] protected Vector3 _selectedScale = new Vector3(1, 1, 1);

        public UnityEvent OnButtonSelected;
        public UnityEvent OnButtonDeslected;

        bool _isClicked = false;
        bool _isSelected = false;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            _normalScale = transform.localScale;
        }

        protected virtual void Start()
        {
            if(_targetImage)
            {
                _targetImage.color = _isSelected ? _selectedColor : _normalColor;
            }
        }

        // 다른 오브젝트들의 OnEnable과 순서가 보장되지 않는다.
        // 최대한 독립적으로 작동하도록 작성할 것.
        private void OnEnable()
        {
            _isClicked = false;
        }

        #region EventSystem Callback
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(_isSelected)
                return;
            
            Select();
        }

        // Hightlighted(Hovered)
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
                Sequence enterSequence = DOTween.Sequence()
                    .Append(_targetImage.transform.DOScale(_highlightScale, TransitionTime))
                    .Join(_targetImage.DOColor(_hightlightedColor, TransitionTime))
                    .SetTarget(transform)
                    .SetUpdate(true);
            }
            // Button was clicked before enter.
            // Go to clicked state
            else if(_targetImage && _isClicked)
            {
                Sequence enterClickSequence = DOTween.Sequence()
                    .Append(_targetImage.transform.DOScale(_pressedScale, TransitionTime))
                    .Join(_targetImage.DOColor(_pressedColor, TransitionTime))
                    .SetTarget(transform)
                    .SetUpdate(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_isSelected)
                return;

            // Go to normal state
            if(_targetImage)
            {
                Sequence exitSequence = DOTween.Sequence()
                    .Append(_targetImage.transform.DOScale(_normalScale, TransitionTime))
                    .Join(_targetImage.DOColor(_normalColor, TransitionTime))
                    .SetTarget(transform)
                    .SetUpdate(true);
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
                Sequence pressedSequence = DOTween.Sequence()
                    .Append(_targetImage.transform.DOScale(_pressedScale, TransitionTime))
                    .Join(_targetImage.DOColor(_pressedColor, TransitionTime))
                    .SetTarget(transform)
                    .SetUpdate(true);
            }
        }
        
        #endregion EventSystem Callback        

        #region Methods
        
        // Select can be called from outside
        public virtual void Select()
        {
            if(_isSelected)
            {
                return;
            }

            _isSelected = true;

            if(_targetImage)
            {
                DOTween.Kill(transform); // DoTween을 Kill하려면 Start 이후에서 호출해야한다.
                _targetImage.transform.localScale = _selectedScale;
                _targetImage.color = _selectedColor;
            }

            OnButtonSelected?.Invoke();
        }

        public virtual void Deselect()
        {
            _isSelected = false;
            _isClicked = false;

            if(_targetImage)
            {
                int killTransform = DOTween.Kill(transform);
                _targetImage.transform.localScale = _normalScale;
                _targetImage.color = _normalColor;
            }
            
            OnButtonDeslected?.Invoke();
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
