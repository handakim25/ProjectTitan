using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    /// <summary>
    /// Player의 데이터를 가지고 처리한다.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public bool IsWalk;

        public void SetLastDirection(Vector3 dir, bool isImmedate = false)
        {
            // player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(faceDir), player.LookRotationDamp * Time.deltaTime);
            
        }
    }
}
