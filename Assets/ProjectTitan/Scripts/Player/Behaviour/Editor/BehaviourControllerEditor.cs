using UnityEngine;
using UnityEditor;

namespace Titan.Character.Player
{
    [CustomEditor(typeof(BehaviourController))]
    public class BehaviourControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BehaviourController controller = target as BehaviourController;

            if(Application.isEditor && Application.isPlaying)
            {
                EditorGUILayout.Space();
                var behaviour = controller.GetCurrentBehaviour();
                var type = behaviour.GetType();
                EditorGUILayout.LabelField("Current State", type.Name);
            }
        }
    }
}
