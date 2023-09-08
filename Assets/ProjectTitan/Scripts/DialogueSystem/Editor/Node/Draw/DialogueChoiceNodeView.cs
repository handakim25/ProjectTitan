using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// 선택지 하나를 나타내는 노드
    /// </summary>
    public class DialogueChoiceNodeView : DialogueBaseNodeView
    {
        public string Sentence;
        protected override void BuildView()
        {
            base.BuildView();

            var outputPort = CreatePort(DialoguePortType.Choice, Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Selector Connection";
            outputPort.portColor = Color.green;
            outputContainer.Add(outputPort);

            var customContainer = new VisualElement();  

            var textFoldout = new Foldout() {text = "Choice Text", value = true};
            var textCountLabel = new Label("text count : 0");
            var sentenceTextField = new TextField() {multiline = true};
            sentenceTextField.RegisterValueChangedCallback(evt =>
            {
                Sentence = evt.newValue;
                textCountLabel.text = $"text count : {evt.newValue.Length}";
            });
            sentenceTextField.SetValueWithoutNotify(Sentence);

            textFoldout.Add(sentenceTextField);
            textFoldout.Add(textCountLabel);
            customContainer.Add(textFoldout);
            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }
    }
}
