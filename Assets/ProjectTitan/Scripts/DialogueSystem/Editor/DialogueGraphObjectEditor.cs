using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Titan.Resource;

namespace Titan.DialogueSystem.Data
{
    // @Note
    // 에디터 관련 코드는 한 곳으로 모으는 것이 맞다.
    // Dialogue Graph Object가 Editor 에서 사용되는 것은 맞지만 일단은 분리하는 것이 맞다.

    /// <summary>
    /// DialogueGraph를 더블클릭해서 열어주게 하거나 Inspector 창에 Open Editor 버튼을 추가하는 커스텀 에디터
    /// 실제 데이터 편집은 DialogueEidtorWindow에서 한다.
    /// </summary>
    [CustomEditor(typeof(DialogueGraphObject))]
    public class DialogueGraphObjectEditor : Editor
    {
        /// <summary>
        /// Dialgoue Graph Object를 선택할 시에 에디터로 열어주는 메뉴를 추가한다.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // for debugging
            base.OnInspectorGUI();

            if(GUILayout.Button("Open Editor"))
            {
                DialogueEditorWindow.Open(target as DialogueGraphObject);
            }
        }

        // https://forum.unity.com/threads/is-it-possible-to-open-scriptableobjects-in-custom-editor-cindows-with-double-click.992796/
        /// <summary>
        /// Dialogue Graph Object를 더블 클릭하면 에디터를 열어준다.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            DialogueGraphObject graph = EditorUtility.InstanceIDToObject(instanceID) as DialogueGraphObject;
            if(graph != null)
            {
                DialogueEditorWindow.Open(graph);
                return true;
            }
            return false;
        }

        [MenuItem("Assets/Create/Dialogue System/New Dialogue Graph")]
        public static void CreateDialogueGraph()
        {
            var graph = CreateInstance<DialogueGraphObject>();

            AssetCreator.CreateAssetInCurrentFolder<DialogueGraphObject>("New Dialogue Graph", (graph) =>
            {
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssetIfDirty(graph);
            });
        }
    }
}
