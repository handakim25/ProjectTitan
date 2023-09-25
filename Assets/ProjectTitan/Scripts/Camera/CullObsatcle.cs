using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    [RequireComponent(typeof(Camera))]
    public class CullObsatcle : MonoBehaviour
    {
        List<GameObject> _prevObstacles = new List<GameObject>();
        List<GameObject> _currentObstacles = new List<GameObject>();
        private GameObject _target;

        private void Awake()
        {
            if(_target == null)
            {
                _target = GameObject.FindGameObjectWithTag("Player");
                if(_target == null)
                {
                    Debug.LogError("Can't find Player");
                }
            }
        }

        private void LateUpdate()
        {
            Ray ray = new(transform.position, _target.transform.position - transform.position);

            // Raycast
            RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, _target.transform.position));
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.CompareTag("Obstacle"))
                {
                    Debug.Log($"Hide {hit.collider.gameObject.name}");
                    _currentObstacles.Add(hit.collider.gameObject);
                    HideObstacle(hit.collider.gameObject);
                }
            }

            var removedObjevcts = _prevObstacles.Except(_currentObstacles);
            foreach(GameObject obj in removedObjevcts)
            {
                Debug.Log($"Show {obj.name}");
                ShowObstacle(obj);
            }
            _prevObstacles = _currentObstacles.ToList();
            _currentObstacles.Clear();
        }

        private void HideObstacle(GameObject obstacle)
        {
            if(obstacle.TryGetComponent<Renderer>(out var renderer))
            {
                var color = renderer.material.color;
                color.a = 0.0f;
                renderer.material.color = color;
            }
        }

        private void ShowObstacle(GameObject obstacle)
        {
            if(obstacle.TryGetComponent<Renderer>(out var renderer))
            {
                var color = renderer.material.color;
                color.a = 1.0f;
                renderer.material.color = color;
            }
        }
    }
}
