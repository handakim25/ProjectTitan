using System.Collections;
using System.Collections.Generic;
using Titan.DialogueSystem.Data.View;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace Titan.DialogueSystem.Data.Nodes
{
    /// <summary>
    /// 대사를 표현하는 노드
    /// </summary>
    // input port : multi, 여러 곳에서 한 대사로 돌아올 수 있다.
    // output port : single, 분기는 할 수 없다. 분기를 하고 싶으면 selector를 이용할 것
    public class DialogueSentenceNodeView : DialogueBaseNodeView
    {
        public string Sentence;

        public PortData inputPortData;
        public PortData outputPortData;

        // Talker
        // Audio

        protected override void BuildView()
        {
            base.BuildView();
            // Load Style sheet
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueSentenceNodeView.uss"));

            // 같은 데이터를 여러 방식으로 사용하는 것은 좋은 설계가 아니다.
            // 추후에 View의 형식에 맞춰서 Slot으로 추상화해서 중복된 코드를 줄이고 데이터의 애매함을 줄여 표현성을 늘릴 것
            var inputPort = CreatePort(DialoguePortType.Dialogue, Direction.Input, Port.Capacity.Multi, ref inputPortData);
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            var outputPort = CreatePort(DialoguePortType.Dialogue, Direction.Output, Port.Capacity.Single, ref outputPortData);
            outputPort.portName = "Next Dialogue";
            outputContainer.Add(outputPort);

            // Custom Container
            var customContainer = new VisualElement();

            var textFoldout = new Foldout() {text = "Dialogue Text", value = true};
            var textCountLabel = new Label($"text count : {Sentence?.Length ?? 0}");
            // 현재 유니티 TextField는 IME 오류가 있어서 문자 조합 중에 제대로 출력이 되지 않는다.
            // 2022 버전부터는 수정되어 있는 문제
            // 직접 수정하기에는 번거로운 문제이므로 버전업을 고려하던지 아니면 그대로 둘 것
            var sentenceTextField = new TextField() {multiline = true};
            // https://forum.unity.com/threads/multiline-text-area-word-wrapping-scrolling-all-that-jazz.826251/
            sentenceTextField.AddToClassList("dialogue--text");
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
