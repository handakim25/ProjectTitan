using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Titan.DialogueSystem;

namespace Titan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueUIScene : UIScene
    {
        [ SerializeField] private float _transitionTime = 0.5f;
        private CanvasGroup _canvasGroup;
        private DialogueUIController dialogueUIConroller;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            dialogueUIConroller = GetComponent<DialogueUIController>();

            Debug.Assert(_canvasGroup != null, "CanvasGroup is null");
            Debug.Log($"UI Manager : {UIManager.Instance}");
        }

        protected override void HandleUIOpen()
        {
            // @Fix
            // Game Stop이 구현되었으므로 SetUpdate를 true로 해야 애니메이션이 진행된다.
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1.0f, 0.0f)
                        .SetDelay(_transitionTime)
                        .SetUpdate(true)
                        .OnComplete(() =>
            {
                dialogueUIConroller._isDialogueAnimating = false;
            });
        }

        protected override void HandleUIClose()
        {
            gameObject.SetActive(false);

            UIManager.Instance.CloseUIScene(this);
        }
    }
}
