using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Titan
{
    public class SlotUI : MonoBehaviour
    {
        /// <summary>
        /// 아이템의 아이콘 이미지
        /// </summary>
        public Sprite IconImage{
            set {
                if(!_icon)
                    return;
                _icon.sprite = value;
            }
        }

        /// <summary>
        /// 아이템 희귀도 색상
        /// </summary>
        public Color RarityColor{
            set {
                if(!_rarityBackground)
                    return;
                _rarityBackground.color = value;
            }
        }

        /// <summary>
        /// 아이템 이름 텍스트
        /// </summary>
        public string ItemNameText{
            set{
                if(!_text)
                    return;
                _text.text = value;
            }
        }

        /// <summary>
        /// 아이템 설명 텍스트
        /// </summary>
        public string ItemDescText {
            set {
                if(!_desc)
                    return;
                _desc.text = value;
            }
        }

        /// <summary>
        /// 아이템의 Subtype을 표시하는 텍스트
        /// </summary>
        public string ItemSubTypeText {
            set {
                if(!_type)
                    return;
                _type.text = value;
            }
        }

        [SerializeField] private Image _icon;
        [SerializeField] private Image _rarityBackground;
        [SerializeField] private Color _rarityColor;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _desc;
        [SerializeField] private TMP_Text _type;
    }
}
