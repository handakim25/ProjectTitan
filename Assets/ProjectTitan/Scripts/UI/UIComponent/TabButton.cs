using UnityEngine;
using UnityEngine.Serialization;

namespace Titan.UI
{
    /// <summary>
    /// Tab 기능을 추가한 Button.
    /// TabGroup을 등록하면 한 번에 하나의 TabButton만 선택될 수 있게 된다.
    /// </summary>
    public class TabButton : TweenButton
    {
        [Tooltip("TabGroup을 등록하면 한 번에 하나의 TabButton만 선택될 수 있게 된다.")]
        [FormerlySerializedAs("tabGroup")]
        [SerializeField] private TabGroup _tabGroup;
        public TabGroup TabGroup
        {
            get => _tabGroup;
            set
            {
                if(_tabGroup != null)
                {
                    _tabGroup.Unsubscribe(this);
                }
                _tabGroup = value;
                if(_tabGroup != null)
                {
                    _tabGroup.Subscribe(this);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            if(_tabGroup != null)
            {
                _tabGroup.Subscribe(this);
            }
        }

        public override void Select()
        {
            base.Select();
            if(_tabGroup != null)
            {
                _tabGroup.SelectTab(this);
            }
        }

        public override void Deselect()
        {
            base.Deselect();
            if(_tabGroup != null)
            {
                _tabGroup.OnTabDeslected(this);
            }
        }
    }
}
