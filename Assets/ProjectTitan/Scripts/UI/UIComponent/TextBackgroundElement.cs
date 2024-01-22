using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Titan
{
    /// <summary>
    /// 현재 사용하지 않는다. Vertical Layout Group을 사용하면 해결할 수 있다.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class TextBackgroundElement : UIBehaviour, ILayoutElement
    {
        [SerializeField] private float _padding = 0;
        TextMeshProUGUI text;
        TextMeshProUGUI Text => text != null ? text : text = GetComponentInChildren<TextMeshProUGUI>();

        public float minWidth => -1;
        public float preferredWidth => Text != null ? Text.preferredWidth + _padding : -1;
        public float flexibleWidth => -1;
        public float minHeight => -1;
        public float preferredHeight => Text != null ? Text.preferredHeight + _padding : -1;
        public float flexibleHeight => -1;
        public int layoutPriority => 1;

        public void CalculateLayoutInputHorizontal() {}
        public void CalculateLayoutInputVertical() {}

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            SetDirty();
            base.OnDisable();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        private void OnTransformChildrenChanged()
        {
            Debug.Log("OnTransformChildrenChanged");
            SetDirty();
        }

        protected void SetDirty()
        {
            // TMPro_EventManager.TEXT_CHANGED_EVENT.Add
            if(!IsActive())
            {
                return;
            }
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}
