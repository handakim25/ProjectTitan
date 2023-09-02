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
    public class DialogueSentenceNode : DialogueBaseNode
    {
        public string Senetence;

        // Talker
        // Audio

        protected override void BuildView()
        {
            base.BuildView();
            // Load Style sheet
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueSentenceNode.uss"));

            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Next Dialogue";
            outputContainer.Add(outputPort);

            // Custom Container
            var customContainer = new VisualElement();

            var textFoldout = new Foldout() {text = "Dialogue Text", value = true};
            var textCountLabel = new Label("text count : 0");
            var sentenceTextField = new TextField() {multiline = true};
            
            sentenceTextField.RegisterCallback<FocusInEvent>(evt => {
                Debug.Log($"Focus in"); Input.imeCompositionMode = IMECompositionMode.On;
            });
            sentenceTextField.RegisterCallback<FocusOutEvent>(evt => {Debug.Log("Focus Out"); Input.imeCompositionMode = IMECompositionMode.Auto;});
            sentenceTextField.RegisterValueChangedCallback(evt =>
            {
                Senetence = evt.newValue;
                Debug.Log($"new value : {evt.newValue}");
            });
            // sentenceTextField.input
            // sentenceTextField.RegisterValueChangedCallback(evt =>
            // {
            //     Senetence = evt.newValue;
            //     textCountLabel.text = $"text count : {evt.newValue.Length}";
            // });
            // Keyboard.current.onTextInput += (char c) =>
            // {
            //     if (sentenceTextField.focusController.focusedElement == sentenceTextField)
            //     {
            //         Senetence += c;
            //         sentenceTextField.value = Senetence;
            //         textCountLabel.text = $"text count : {Senetence.Length}";
            //     }
            // };
            // string test = Input.compositionString;
            // Keyboard.current.onIMECompositionChange += (UnityEngine.InputSystem.LowLevel.IMECompositionString evt) =>
            // {
            //     if (sentenceTextField.focusController.focusedElement == sentenceTextField)
            //     {
            //         // evt.
            //         sentenceTextField.
            //     }
            // };
            
            textFoldout.Add(sentenceTextField);
            textFoldout.Add(textCountLabel);
            customContainer.Add(textFoldout);
            extensionContainer.Add(customContainer);

            RefreshExpandedState();
        }
    }
}
