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

        public string ItemDescText {
            set {
                if(!_desc)
                    return;
                _desc.text = value;
            }
        }

        public string ItemTypeText {
            set {
                if(!_type)
                    return;
                _type.text = value;
            }
        }

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _desc;
        [SerializeField] private TMP_Text _type;
    }
}
