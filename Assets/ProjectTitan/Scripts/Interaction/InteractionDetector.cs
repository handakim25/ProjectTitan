using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Interaction
{
    // Interaction 가능 List를 찾기 위한 Detector
    /// <summary>
    /// Detect Interval마다 주변에 상호작용 가능한 Interactable을 찾아서 Interaction List를 Update한다.
    /// </summary>
    public class InteractionDetector : MonoBehaviour
    {
        [Tooltip("Interaction List")]
        [SerializeField] private InteractionList _interactionList;
        [Tooltip("탐색 간격")]
        [SerializeField] private float _detectInterval = 0.1f;
        [Tooltip("탐색 반경")]
        [Range(0.1f, 10f)]
        [SerializeField] private float _detectRadius = 5f;
        [Tooltip("탐색 대상 레이어")]
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
            if(!gameObject.activeInHierarchy)
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
            Interactable[] interactables = new Interactable[_interactionList.MaxInteractObjects];
            while(true)
            {
                int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, _detectRadius, colliders, _targetMask);
                int interactableCount = 0;
                for(int i = 0; i < colliderCount; i++)
                {
                    if(colliders[i].gameObject.TryGetComponent<Interactable>(out var interactable)
                        && interactable.CanInteract)
                    {
                            interactables[i] = interactable;
                            interactableCount++;
                    }
                }
                _interactionList.UpdateInteractionList(interactables, interactableCount);
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
