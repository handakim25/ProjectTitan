using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Titan
{
    public class SlotUI : MonoBehaviour
    {
        public Sprite IconImage{
            set {
                if(!_icon)
                    return;
                _icon.sprite = value;
            }
        }

        public string ItemNameText{
            set{
                if(!_text)
                    return;
                _text.text = value;
            }
        }

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;
    }
}
