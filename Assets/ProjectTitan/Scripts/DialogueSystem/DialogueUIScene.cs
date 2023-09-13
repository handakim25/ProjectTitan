using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Titan.DialogueSystem;

namespace Titan.UI
{
    public class DialogueUIScene : UIScene
    {
        [ SerializeField] private float _transitionTime = 0.5f;
        private CanvasGroup _canvasGroup;
        private DialogueUIController dialogueUIConroller;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            dialogueUIConroller = GetComponent<DialogueUIController>();
        }

        public override void OpenUI()
        {
            gameObject.SetActive(true);

            UIManager.Instance.OpenUIScene(this);

            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1.0f, 0.0f).SetDelay(_transitionTime).OnComplete(() =>
            {
                dialogueUIConroller._isDialogueAnimating = false;
            });
        }

        public override void CloseUI()
        {
            gameObject.SetActive(false);

            UIManager.Instance.CloseUIScene(this);
        }
    }
}
