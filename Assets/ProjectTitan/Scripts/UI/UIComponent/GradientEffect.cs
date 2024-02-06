using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Titan
{
    public class GradientEffect : BaseMeshEffect
    {
        public Color topLeftColor = Color.white;
        public Color topRightColor = Color.white;
        public Color bottomLeftColor = Color.white;
        public Color bottomRightColor = Color.white;

        // See BaseMeshEffect.cs
        // See Graphic.cs
        // See Image.cs
        // See Default UI Shader
        // Image에서 OnPopulateMesh를 호출하고 Mesh 생성 후에 IMeshModifier들을 호출한다.
        // Default UI Shader에서는 Vertex Color를 Vertex Shader에서 넘겨준다.
        // Vertex Shader결과물은 보간되어서 Fragment Shader로 넘어간다.
        // 한계점 : vertex shader에서 보간되어 버리기 때문에 보간 방법을 바꿀 수 없다.
        // 만약 Fragment Shader를 직접 작성한다면 uv 좌표를 기반으로 보간을 할 수 있다.
        // Gradient를 직접 입력 받아서 임시 Texture로 만든다음에 이를 넘겨서 보간한다면?
        // Gradient를 Soft Mask로 만들어서 적용하면 비슷한 효과가 날것으로 추정된다.
        // Fragment Shader의 보간 방법은 밑의 자료 참고
        // https://learn.microsoft.com/ko-kr/windows/win32/direct3d11/pixel-shader-stage
        public override void ModifyMesh(VertexHelper vh)
        {
            var verts = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);

            // 1st version
            // 단순하게 사각형의 꼭지점 색을 기준으로 선형 보간을 한다.
            for (int i = 0; i < verts.Count; i++)
            {
                UIVertex uiVertex = verts[i];
                float lerpValueX = uiVertex.uv0.x; // Use the u-coordinate of the UV as the lerp value
                float lerpValueY = uiVertex.uv0.y; // Use the v-coordinate of the UV as the lerp value

                Color topColor = Color.Lerp(topLeftColor, topRightColor, lerpValueX);
                Color bottomColor = Color.Lerp(bottomLeftColor, bottomRightColor, lerpValueX);
                uiVertex.color = Color.Lerp(bottomColor, topColor, lerpValueY);
                verts[i] = uiVertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            ListPool<UIVertex>.Release(verts);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            topLeftColor = graphic.color;
            topRightColor = graphic.color;
            bottomLeftColor = graphic.color;
            bottomRightColor = graphic.color;
        }
#endif        
    }
}
