using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

using Titan.QuestSystem;


namespace Titan.DialogueSystem.Data.Nodes
{
 
 
    public class DialogueQuestNodeView : DialogueConditionNodeView
    {
        /// <summary>
        /// Quest SO GUID
        /// </summary>
        public string QuestSOGUID;
        /// <summary>
        /// Quest의 ID
        /// </summary>
        public string TriggerQuestID;
        /// <summary>
        /// Quest를 어떻게 상태 변화시킬지
        /// </summary>
        public QuestStatus QuestStatus = QuestStatus.NotReceived;

        protected override void BuildView()
        {
            base.BuildView();

            var customContainer = new VisualElement();

            var questFoldOut = new Foldout() {text = "Quest", value = true};

            var questStatusField = new EnumField("Quest Status", QuestStatus);
            questStatusField.RegisterValueChangedCallback(evt => QuestStatus = (QuestStatus) evt.newValue);

            var questField = new ObjectField() {objectType = typeof(QuestObject)};
            if(string.IsNullOrEmpty(QuestSOGUID) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(QuestSOGUID);
                var quest = AssetDatabase.LoadAssetAtPath<QuestObject>(assetPath);
                questField.SetValueWithoutNotify(quest);
                questStatusField.SetEnabled(true);
            }
            else
            {
                questStatusField.SetEnabled(false);
            }
            questField.RegisterValueChangedCallback(evt =>
            {
                var quest = evt.newValue as QuestObject;
                if(quest != null)
                {
                    questStatusField.SetEnabled(true);
                    TriggerQuestID = quest.Quest.QuestID;
                    var assetPath = AssetDatabase.GetAssetPath(quest);
                    QuestSOGUID = AssetDatabase.AssetPathToGUID(assetPath);
                }
                else
                {
                    questStatusField.SetEnabled(false);
                    TriggerQuestID = null;
                    QuestSOGUID = null;
                }
            });

            questFoldOut.Add(questField);
            questFoldOut.Add(questStatusField);
            customContainer.Add(questFoldOut);

            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }

        public override Requirement GetRequirement()
        {
            return new Requirement()
            {
                Type = Requirement.RequirementType.Quest,
                ExpectedBool = true,
                TargetID = TriggerQuestID,
                QuestStatus = QuestStatus,
            };
        }
    }
}
