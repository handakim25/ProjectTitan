using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Titan.DialogueSystem.Data.Nodes
{
    public class DialogueAnyNodeView : DialougeLogicNodeView
    {
        protected int _conditionCount;

        protected override void BuildView()
        {
            base.BuildView();

            var conditionAddContainer = new VisualElement() { name = "condition-add" };

            var conditionAddButton = new Button(() =>
            {
                AddCondition();
            })
            { text = "Add Condition" };

            conditionAddContainer.Add(conditionAddButton);
            topContainer.parent.Add(conditionAddContainer);
            topContainer.PlaceInFront(conditionAddContainer);

            var divider = new VisualElement() {name = "divider"};
            divider.AddToClassList("horizontal");
            topContainer.parent.Add(divider);
            divider.PlaceInFront(conditionAddContainer);

            for(int i = 0; i < 1; i++)
            {
                AddCondition();
            }
        }

        private void AddCondition()
        {
            _conditionCount++;

            var inputLogic = CreatePort(DialoguePortType.Condition, Direction.Input, Port.Capacity.Single);
            inputLogic.portName = "Condition";
            
            {
                var deleteButton = new Button(() =>
                {
                    if (_conditionCount <= 1)
                    {
                        return;
                    }
                    if (inputLogic.connected)
                    {
                        // Disconnect할 필요는 없나?
                        _graphView.DeleteElements(inputLogic.connections);
                    }
                    inputContainer.Remove(inputLogic);
                    _conditionCount--;
                })
                { text = "X" };

                inputLogic.Add(deleteButton);
            }

            inputContainer.Add(inputLogic);
        }
    }
}
