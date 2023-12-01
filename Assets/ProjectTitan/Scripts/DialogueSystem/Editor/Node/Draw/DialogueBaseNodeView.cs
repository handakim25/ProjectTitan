using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace Titan.DialogueSystem.Data.Nodes
{
    using System;
    using UnityEditor;
    using View;

    // @refacotr
    // 1. Port 분리
    // 2. Load View, Create View 상황 관리

    /// <summary>
    /// Dialogue System에 사용되는 Node들이 상속 받을 클래스
    /// View에 해당된다. 각각 필요한 데이터는 직렬화되어 있으며 BuildView를 실행할 때 자기 자신의 View를 생성해야한다.
    /// </summary>
    // Shader Graph에서는 AbstractMaterialNode가 데이터로 존재하고 이것을 리플렉션으로 MaterialNodeView 로 생성을 해주는 방식이다. 잠깐 근데 일부 틀린 곳이 존재. 추후 보강할 것.
    // 한 마디로 AbstractMaterilaNode는 Model이고 MaterialView는 View, GraphData가 Controller 역할을 한다.
    // Controller를 조종하는 것은 GraphEditorView이다.
    // 하지만 이것을 고려해서 작성하기에는 시간도 짧고 완성하기에는 많은 시간이 걸린다.
    // 따라서 구현을 간단화 하기 위해서 몇 가지 기준을 세우기로 했다.
    // 1. DialgoueBaeNode는 Concrete Class들로 나누어서 구현한다. 이것이 가능한 이유는 Shader Graph처럼 수많은 Node가 존재하지 않기 때문이다.
    // 하지만 여전히 Shader Graph의 모델은 좋은 것이 Abstract Material Node만 관리하면 View는 알아서 관리되기 때문이다.
    // 지금 내 시스템은 그래프 상의 Node와 Serialize하기 위한 Node 타입 하나, Run-time에서 사용하기 위한 node 이렇게 3개를 관리해야 한다.
    // 실수를 안 할 수가 없는 시스템인 것이 문제이다. 하나의 변경 사항이 3 클래스를 수정해야 하는 좋지 않은 케이스
    // 2. Data Node는 추가될 때마다 바로 관리 되는 것이 아니라 Save가 발생하면 그 때의 Snapshot을 기준으로 저장을 한다.
    // 따라서 un-do를 지원하지 않는다.
    public abstract class DialogueBaseNodeView : Node
    {
        /// <summary>
        /// Node의 ID. GUID를 사용한다. 직렬화될 때 사용된다.
        /// </summary>
        [SerializeField] private string _id;
        public string ID
        {
            get => _id;
            protected set => _id = value;
        }

        /// <summary>
        /// Graph에서 View의 위치
        /// </summary>
        [SerializeField] protected Vector2 _pos;

        /// <summary>
        /// Node View가 가지고 있는 Port 리스트
        /// </summary>
        protected DialogueGraphView _graphView;

        /// <summary>
        /// View를 초기화한다. 새로 생성된 경우 id를 생성하고, 로드하는 경우 id를 기반으로 load한다.
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="id">null일 경우 새로 생성, 로드하는 과정일 경우 id를 기반으로 load</param>
        public void Initialize(DialogueGraphView graphView, string id = null)
        {
            if(string.IsNullOrEmpty(id))
            {
                ID = Guid.NewGuid().ToString();
            }
            else
            {
                ID = id;
            }
            _graphView = graphView;

            // Load style sheet
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueEditorWindow.StyleSheetsPath + "DialogueBaseNodeView.uss"));
            AddToClassList("dialogue--node");

            // Set Title Text
            string typeName = GetType().Name;
            typeName = typeName.Replace("Dialogue", "").Replace("NodeView", "");
            title = typeName;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            if(_pos != Vector2.zero)
            {
                SetPosition(new Rect(_pos, Vector2.zero));
            }

            BuildView();
        }

        #region Container Structure

        // Container 구성
        // - content container : element 자신과 유사하다. node-border, selection-border를 가지고 있다. node-border은 main container이다.
        // - main container : 모든 내용을 담고 있는 container. title, contents, ""이 있다.
        // - title container : title 이름이 들어가 있는 container
        // - top contaienr : title container 아래에 위치한 container. input container, output container가 위치한다.
        // - input container : top container의 왼쪽에 위치한 container
        // - output container : top container의 오른쪽에 위치한 container
        // - extension container : custom elements를 위한 container

        // main : child 3 / title, contents, ""

        // - title
        // -- title-label
        // -- title-button-container
        // --- collapse-button
        // ---- icon

        // - contents : child 3 / divider, top, collapsible-area
        // -- divider
        // -- top
        // --- input
        // ---- ""
        // --- divider
        // --- output
        // ---- ""
        // --- ""
        // -- collapsible-area
        // --- divider
        // --- extension
        // ---- ""

        // - ""

        // content container : child 2
        // - node-border(main container)
        // -- title
        // --- title-label
        // ...
        // - selection-border

        #endregion Container Structure

        /// <summary>
        /// View를 생성
        /// </summary>
        protected virtual void BuildView()
        {
        }

        #region Port
        
        public enum DialoguePortType
        {
            None,
            Dialogue,
            Choice,
            Condition,
        }

        // Port 관련 데이터를 나중에 분리할 것
        
        /// <summary>
        /// Port를 직렬화하기 위한 데이터
        /// </summary>
        [Serializable]
        public class PortData
        {
            /// <summary>
            /// Port의 ID
            /// </summary>
            public readonly string PortID;
            /// <summary>
            /// 연결된 Port의 ID, 연결이 없을 경우 null or empty
            /// </summary>
            public string ConnectedPortID;
            /// <summary>
            /// 연결되기 위한 Port의 종류
            /// </summary>
            public DialoguePortType PortType;
            /// <summary>
            /// Port가 속한 Node의 ID
            /// </summary>
            public string nodeId;

            public PortData(string PortID, DialoguePortType PortType, DialogueBaseNodeView node)
            {
                this.PortID = PortID;
                this.PortType = PortType;
                ConnectedPortID = null;
                nodeId = node.ID;
            }

            public override string ToString()
            {
                return $"PortID : {(string.IsNullOrEmpty(PortID) ? "NULL" : PortID)} / ConnectedPortID : {ConnectedPortID} / PortType : {PortType} / NodeID : {nodeId}";
            }
        }

        [Serializable]
        public class PortDataPair
        {
            public PortData inputPortData;
            public PortData outputPortData;
        }  

        // Port를 재정의하기에는 너무 번거로운 면이 있다.
        // 현재로는 userData를 이용하고 추후에 추가적인 기능이 필요하면 상속 받아서 사용할 것
        // 참고 링크
        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Elements/Port.cs#L243
        // https://forum.unity.com/threads/graphview-inheriting-from-port-to-store-custom-data.1161392/
        // https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.shadergraph/Editor/Drawing/Views/ShaderPort.cs#L73
        /// <summary>
        /// Port를 생성한다. 만약에 Load와 같이 PortData가 존재할 경우 PortData를 이용해서 생성한다.
        /// 없을 경우, 새로 생성한다.
        /// </summary>
        /// <param name="type">허용되는 Port Type</param>
        /// <param name="direction">현재는 수평 방향만 사용</param>
        /// <param name="capacity">연결될 수 있는 Port 개수설정</param>
        /// <param name="portData">로드 시에 PortData를 이용해서 생성</param>
        /// <returns>생성된 Port</returns>
        protected Port CreatePort(DialoguePortType type, Direction direction, Port.Capacity capacity, ref PortData portData)
        {
            var port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
            portData ??= new PortData(Guid.NewGuid().ToString(), type, this);
            port.userData = portData;
            port.portColor = GetPortColor(type);

            return port;
        }

        /// <summary>
        /// Port를 새로 생성한다.
        /// </summary>
        /// <param name="type">허용되는 Port Type</param>
        /// <param name="direction">현재는 수평 방향만 사용</param>
        /// <param name="capacity">연결될 수 있는 Port 개수설정</param>
        /// <returns>생성된 Port</returns>
        protected Port CreatePort(DialoguePortType type, Direction direction, Port.Capacity capacity)
        {
            var port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
            port.userData = new PortData(Guid.NewGuid().ToString(), type, this);
            port.portColor = GetPortColor(type);

            return port;
        }

        /// <summary>
        /// Porrt 연결이 가능한지 확인한다. 현재는 Port Type이 같을 경우에 True
        /// </summary>
        /// <param name="start">시작 Port</param>
        /// <param name="end">끝 Port</param>
        /// <returns>True일 경우 연결 가능, False일 경우 불가능</returns>
        public static bool CanConnect(Object start, Object end)
        {
            if(start == null || end == null || start == end)
            {
                return false;
            }

            var startType = start is PortData startPortData ? startPortData.PortType : DialoguePortType.None;
            var endType = start is PortData endPortData ? endPortData.PortType : DialoguePortType.None;

            return startType == endType;
        }

        /// <summary>
        /// Port를 연결한다.
        /// </summary>
        /// <param name="input">도착 Port(들어오는 포트)</param>
        /// <param name="output">출발 Port(나가는 포트)</param>
        public static void Connect(Port input, Port output)
        {
            // input port가 도착, output port가 출발이란 것에 주의
            var inputPortData = input.userData as PortData;
            var outputPortData = output.userData as PortData;
            outputPortData.ConnectedPortID = inputPortData.PortID;
            inputPortData.ConnectedPortID = outputPortData.PortID;
        }

        /// <summary>
        /// Port 연결을 끊는다.
        /// </summary>
        /// <param name="input">도착 Port(들어오는 Port)</param>
        /// <param name="output">출발 Port(나가는 포트)</param>
        public static void Disconnect(Port input, Port output)
        {
            var outputPortData = output.userData as PortData;
            var inputPortData = input.userData as PortData;
            outputPortData.ConnectedPortID = null;
            inputPortData.ConnectedPortID = null;
        }

        /// <summary>
        /// Port Type에 따라 Port의 색상을 반환한다.
        /// </summary>
        /// <param name="type">Port Type</param>
        /// <returns></returns>
        private Color GetPortColor(DialoguePortType type)
        {
            return type switch
            {
                DialoguePortType.Dialogue => new Color(0.8f, 0.8f, 0.8f),
                DialoguePortType.Choice => Color.green,
                DialoguePortType.Condition => Color.red,
                _ => Color.blue,
            };
        }
        
        #endregion Port

        #region Callback
        
        /// <summary>
        /// 위치 변경을 감지해서 위치를 저장한다.
        /// </summary>
        /// <param name="evt"></param>
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _pos = GetPosition().position;
        }
        
        #endregion Callback
    }
}
