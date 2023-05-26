using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    /// <summary>
    /// 행동들이 관리하는 것은 크게 3가지이다.
    /// 1. 상황에 맞는 적절한 애니메이션 재생
    /// 2. 상황에 따른 Input handling, 전환의 경우는 각자의 상태가 감시한다.
    /// 3. 상황에 맞는 이벤트 발생. 이동이라면 이동, 공격이라면 공격, 상호작용이라면 상호작용
    /// </summary>
    public abstract class GenericBehaviour : MonoBehaviour
    {
        protected BehaviourController _controller;
        protected int _behaviourCode;
        public int BehavriouCode => _behaviourCode;

        private void Awake()
        {
            _controller = GetComponent<BehaviourController>();

            _behaviourCode = GetType().GetHashCode();
        }

        public virtual void LocalUpdate() {}

        public virtual void LocalFixedUpdate() {}

        public virtual void LocalLateUpdate() {}

        /// <summary>
        /// Called when overrided
        /// </summary>
        public virtual void OnOverride() {}

        public virtual void OnEnter() {}
    }
}
