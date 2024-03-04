using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Titan.Core.Scene;
using Titan.Audio;

namespace Titan
{
    // @Refactor
    // UI Manager가 게임 부분만 관리하고 있다. 이는 일관적이지 않으므로 추후에 통합적으로 작동할 수 있도록 수정할 것

    /// <summary>
    /// Title Scene Controller.
    /// Title Scene은 현재 Title 이미지만 보여주고 다른 기능은 하지 않는다.
    /// Click을 하면 Game Scene으로 이동
    /// 시스템 요소들이 존재하는 것을 가정한다.
    /// </summary>
    public class TitleSceneController : MonoBehaviour, IPointerClickHandler
    {
        [Header("Title Scene Setting")]
        [SerializeField] private Sprite _titleSprite;
        [SerializeField] private SoundList _titleSound;
        [SerializeField] private SceneList _gameScene;

        [Header("Title Scene UI Elements")]
        [SerializeField] private Image _titleImage;

        private void Awake()
        {
            if(_titleImage == null)
            {
                _titleImage = transform.Find("TitleImage").GetComponent<Image>();
            }
        }

        private void Start()
        {
            SceneLoadManager.Instance.SceneLoaded += () => {
                if(_titleSprite != null)
                {
                    _titleImage.sprite = _titleSprite;
                }
                SoundManager.Instance.PlayBGM((int)_titleSound);
            };
        }

        private void SetTitleScene(Sprite titleSprite, SoundList titleSound)
        {
            _titleSprite = titleSprite;
            _titleSound = titleSound;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SceneLoadManager.Instance.LoadScenes(_gameScene);            
        }
    }
}
