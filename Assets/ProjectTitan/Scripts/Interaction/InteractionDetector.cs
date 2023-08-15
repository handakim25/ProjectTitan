using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Interaction
{
    // Interaction 가능 List를 찾기 위한 Detector
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private InteractionList _interactionList;
        [SerializeField] private float _detectInterval = 0.1f;
        [SerializeField] private float _detectRadius = 5f;
        [SerializeField] private LayerMask _targetMask;
        
        private Coroutine _detectCoroutine;
        // Default : Active when OnEnable
        public bool IsAcitive {get; protected set;} = true;

        // a. OnEnable && true -> Start Coroutine
        // b. OnEnable && false -> Do not start coroutine
        // c. After OnEnable && Set true -> Start Coroutine
        // d. After OnEnable && Set false -> If Coroutine started, stop coroutine
        private void OnEnable()
        {
            if(IsAcitive && _detectCoroutine == null)
                _detectCoroutine = StartCoroutine(DetectInteract());
        }

        private void OnDisable()
        {
            if(_detectCoroutine != null)
            {
                StopCoroutine(_detectCoroutine);
                _detectCoroutine = null;
            }
        }

        public void SetActive(bool isActive)
        {
            if(isActive == IsAcitive)
                return;
            IsAcitive = isActive;
            if(!gameObject.activeInHierarchy) // 조건이 잘못 들어간 것으로 추정. 버그가 발생한다면 원복할 것
                return;

            if(isActive)
            {
                _detectCoroutine = StartCoroutine(DetectInteract());
            }
            else 
            {
                StopCoroutine(_detectCoroutine);
                _detectCoroutine = null;
            }
        }

        IEnumerator DetectInteract()
        {
            var wait = new WaitForSeconds(_detectInterval);
            Collider[] colliders = new Collider[_interactionList.MaxInteractObjects];
            GameObject[] interactObjects = new GameObject[_interactionList.MaxInteractObjects];
            while(true)
            {
                int numCollider = Physics.OverlapSphereNonAlloc(transform.position, _detectRadius, colliders, _targetMask);
                for(int i = 0; i < numCollider; i++)
                {
                    interactObjects[i] = colliders[i].gameObject;
                }
                _interactionList.UpdateInteractionList(interactObjects, numCollider);
                yield return wait;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectRadius);
        }
    }
}
