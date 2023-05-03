using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using DG.Tweening;

namespace Titan.UI
{
    

    public class TabButton : TweenButton
    {
        public TabGroup tabGroup;

        protected override void Start()
        {
            base.Start();
            tabGroup?.Subscribe(this);
        }

        public override void Select()
        {
            base.Select();
            tabGroup?.OnTabSelected(this);
        }

        public override void Deselect()
        {
            base.Deselect();
            tabGroup?.OnTabDeslected(this);
        }
    }
}
