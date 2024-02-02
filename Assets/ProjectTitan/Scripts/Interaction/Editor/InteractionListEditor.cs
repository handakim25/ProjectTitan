using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan.Interaction
{
    [CustomEditor(typeof(InteractionList))]
    public class InteractionListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isEditor && Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Run-time Information", EditorStyles.boldLabel);
                var interactionList = target as InteractionList;
                foreach(var interaction in interactionList.interactObjects)
                {
                    EditorGUILayout.LabelField(interaction.ToString());
                }
            }
        }
    }
}
