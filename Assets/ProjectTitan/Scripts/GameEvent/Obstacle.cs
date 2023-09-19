using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace Titan
{
    public class Obstacle : MonoBehaviour
    {
        public bool IsOpen = false;
        public void Open()
        {
            if(IsOpen == true) return;
            Debug.Log($"Open Obstacle {gameObject.name}");

            transform.DOMove(transform.position + Vector3.up * 3, 3.0f).OnComplete(() => {
                IsOpen = true;
            });
            
        }

        public void Close()
        {
            if(IsOpen == false) return;

            transform.DOMoveY(3.0f, 1.0f, true).OnComplete(() => {
                IsOpen = false;
            });
        }
    }
}
