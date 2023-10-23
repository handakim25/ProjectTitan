using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using Titan.Audio;

namespace Titan
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private bool _isOpen = false;

        [Header("Animation")]
        [SerializeField] private Vector3 _openOffset;
        [SerializeField] Ease openEase = Ease.InOutSine;
        [SerializeField] Ease closeEase = Ease.InOutSine;
        [SerializeField] private float _openDuration = 1f;
        [SerializeField] private float _closeDuration = 1f;

        [SerializeField] private SoundList _openSound = SoundList.None;
        [SerializeField] private SoundList _closeSound = SoundList.None;

        private Vector3 _originPos;
        private Vector3 _openPos => _originPos + _openOffset;

        [Header("Event")]
        public UnityEvent OnClose;
        public UnityEvent OnOpen;        

        private void Awake()
        {
            _originPos = transform.position;
        }

        private void Start()
        {
            if(_isOpen == true)
            {
                transform.position = _openPos;
            }
        }

        [ContextMenu("Open")]
        public void Open()
        {
            if(_isOpen == true)
            {
                return;
            }
            _isOpen = true;
            SoundManager.Instance.PlayeEffectSound((int)_openSound, transform.position);
            transform.DOMove(_openPos, 1f).SetEase(openEase);
        }

        [ContextMenu("Close")]
        public void Close()
        {
            if(_isOpen == false)
            {
                return;
            }
            _isOpen = false;
            SoundManager.Instance.PlayeEffectSound((int)_closeSound, transform.position);
            transform.DOMove(_originPos, 1f).SetEase(closeEase);
        }
    }
}
