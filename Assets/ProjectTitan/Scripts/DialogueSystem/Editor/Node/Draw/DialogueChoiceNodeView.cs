using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using Titan.DialogueSystem;
using UnityEditor.UIElements;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// 선택지 하나를 나타내는 노드
    /// </summary>
    public class DialogueChoiceNodeView : DialogueBaseNodeView
    {
        /// <summary>
        /// 선택지 문장
        /// </summary>
        public string Sentence;
        /// <summary>
        /// 선택지 Icon 표시
        /// </summary>
        public ChoiceType choiceType = ChoiceType.ContinueChoice;

        /// <summary>
        /// 선택지 Port 정보
        /// </summary>
        public PortData choicePortData;

        protected override void BuildView()
        {
            base.BuildView();

            var outputPort = CreatePort(DialoguePortType.Choice, Direction.Output, Port.Capacity.Single, ref choicePortData);
            outputPort.portName = "Selector Connection";
            outputPort.portColor = Color.green;
            outputContainer.Add(outputPort);

            var customContainer = new VisualElement();  

            var choiceFoldOut = new Foldout() {text = "Choice", value = true};

            var textCountLabel = new Label($"text count : {Sentence?.Length ?? 0}");
            var sentenceTextField = new TextField() {multiline = true};
            sentenceTextField.RegisterValueChangedCallback(evt =>
            {
                Sentence = evt.newValue;
                textCountLabel.text = $"text count : {evt.newValue.Length}";
            });
            sentenceTextField.SetValueWithoutNotify(Sentence);

            choiceFoldOut.Add(sentenceTextField);
            choiceFoldOut.Add(textCountLabel);

            var choiceTypeEnumField = new EnumField("Choice Type", choiceType);
            choiceTypeEnumField.Init(choiceType);
            choiceTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                choiceType = (ChoiceType) evt.newValue;
            });
            choiceFoldOut.Add(choiceTypeEnumField);

            customContainer.Add(choiceFoldOut);
            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }
    }
}
