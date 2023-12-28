using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Battle
{
    /// <summary>
    /// Physics Overlap에 사용할 객체
    /// 현재는 Box 형태만 사용
    /// </summary>
    public class AttackCollider : MonoBehaviour
    {
        public Vector3 Center;
        public Vector3 Size;

        /// <summary>
        /// 정해진 박스에 따라 충돌 판정을 실행
        /// </summary>
        /// <param name="layerMask">충돌 판정을 할 layer mask </param>
        /// <returns>충돌한 객체들</returns>
        public Collider[] CheckOverlap(LayerMask layerMask)
        {
            return Physics.OverlapBox(transform.TransformPoint(Center), Size * 0.5f, transform.rotation, layerMask);
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Center, Size);
        }
    }
}
