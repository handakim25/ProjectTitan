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
    /// CharacterController의 랩퍼이다.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        public float Speed {get; set;}
        /// <summary>
        /// Normalized Move Vector
        /// </summary>
        public Vector3 MoveDir {get; set;}

        protected CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void Move()
        {
            Vector3 Velocity = new Vector3();
            Velocity.x = MoveDir.x * Speed;
            Velocity.z = MoveDir.z * Speed;
            _characterController.Move(Velocity * Time.deltaTime);
        }
    }
}
