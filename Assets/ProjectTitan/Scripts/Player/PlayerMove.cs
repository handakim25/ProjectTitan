using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character.Player
{
    /// <summary>
    /// Player의 위치를 조절하는 컴포넌트
    /// 속도와 방향을 가진다.
    /// 다른 컴포넌트에서 속도, 방향을 설정하고
    /// 이를 바탕으로 업데이트한다.
    /// BehaviourController가 Update Loop에서 사용
    /// CharacterController의 랩퍼이다.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _gravity = -9.8f;
        [SerializeField] private float _fallMultiplier = 2f;
        [SerializeField] private float _terminalSpeed = 20f;

        // @refactor
        // Move to editor code
        [field : SerializeField] public float Speed {get; set;}
        /// <summary>
        /// Normalized Move Vector
        /// </summary>
        public Vector3 MoveDir {get; set;}
        /// <summary>
        /// Whether Apply Gravity or not
        /// </summary>
        /// <value></value>
        public bool IsApplyGravity {get; set;}
        // @refactor
        // Move to editor code
        [SerializeField] private float _fallSpeed;

        protected CharacterController _characterController;

#if UNITY_EDITOR
        public bool DebugMode = true;
#endif

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void SetYSpeed(float speed) => _fallSpeed = speed;
        public void ResetFall() => _fallSpeed = 0;

        public void Move()
        {
            var Velocity = new Vector3
            {
                x = MoveDir.x * Speed,
                z = MoveDir.z * Speed
            };

            if (IsApplyGravity)
            {
                UpdateFallSpeed();
                Velocity.y = _fallSpeed;
            }

#if UNITY_EDITOR
            if(DebugMode)
            {
                Debug.Log($"Player Move : {Velocity}");
            }
#endif
            _characterController.Move(Velocity * Time.deltaTime);
        }

        private void UpdateFallSpeed()
        {
            if(_fallSpeed > 0)
            {
                _fallSpeed += _gravity * Time.deltaTime;
            }
            else if(_fallSpeed > - _terminalSpeed)
            {
                _fallSpeed += _gravity * _fallMultiplier * Time.deltaTime;
            }
        }
    }
}
