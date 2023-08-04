using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.Character.Enemy
{
    [CustomEditor(typeof(StateController))]
    public class StateControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var controller = target as StateController;

            if(Application.isEditor && Application.isPlaying)
            {
                EditorGUILayout.Space();
            }
        }

        private void OnSceneGUI()
        {
            var controller = target as StateController;
            if(controller == null || controller.gameObject == null)
            {
                Debug.LogError($"Can this happen?");
                return;
            }

            if(controller.GeneralStats == null)
            {
                return;
            }
            Handles.color = Color.red;
            Handles.DrawWireArc(controller.transform.position, Vector3.up, Vector3.forward, 360, controller.GeneralStats.PerceptionRadius);
            Handles.DrawWireArc(controller.transform.position, Vector3.up, Vector3.forward, 360, controller.GeneralStats.NearRadius);

            Vector3 viewAngleLeft = DirFromAngle(controller.transform, -controller.GeneralStats.ViewAngle /2 , false);
            Vector3 viewAngleRight = DirFromAngle(controller.transform, controller.GeneralStats.ViewAngle / 2, false);
            Handles.DrawWireArc(controller.transform.position, Vector3.up, viewAngleLeft, controller.GeneralStats.ViewAngle, controller.GeneralStats.ViewRadius);
            Handles.DrawLine(controller.transform.position, controller.transform.position + viewAngleLeft * controller.GeneralStats.ViewRadius);
            Handles.DrawLine(controller.transform.position, controller.transform.position + viewAngleRight * controller.GeneralStats.ViewRadius);
        }

        private Vector3 DirFromAngle(Transform transform, float angleInDrees, bool isAngleGlobal)
        {
            if(!isAngleGlobal)
            {
                angleInDrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDrees * Mathf.Deg2Rad));
        }
    }
}
