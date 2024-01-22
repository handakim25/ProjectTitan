using System;
using UnityEngine;
using UnityEngine.UI;

namespace Titan
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class CustomRect : Graphic
    {
        public float Thickness = 5;
        public bool IsFilled = false;
        [Tooltip("Left(X), Top(Y), Right(Z), Bottom(W)")]
        public Vector4 Margin = Vector4.zero;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            float outerX = -rectTransform.pivot.x * width + Margin.x;
            float outerY = -rectTransform.pivot.y * height + Margin.w;
            // outerX, outerY는 원래의 width, height로 계산해야 구할 수 있다.
            // outerX, outerY를 구하고 나서 margin을 빼줘야 한다.
            width -= Margin.x + Margin.z;
            height -= Margin.y + Margin.w;
            float innerX = outerX + Thickness;
            float innerY = outerY + Thickness;

            if(IsFilled)
            {
                PopulateFilledRect(vh, width, height, outerX, outerY);
            }
            else
            {
                PopulateOutlineRect(vh, width, height, outerX, outerY, innerX, innerY);
            }
        }

        private void PopulateFilledRect(VertexHelper vh, float width, float height, float startX, float startY)
        {
            // 외부 사각형의 버텍스 추가, Vertex 0~3
            vh.AddVert(new Vector3(startX, startY), color, new Vector2(0f, 0f)); // Vertex 0, 왼쪽 아래
            vh.AddVert(new Vector3(startX + width, startY), color, new Vector2(0f, 1f)); // Vertex 1, 오른쪽 아래
            vh.AddVert(new Vector3(startX + width, startY + height), color, new Vector2(1f, 1f)); // Vertex 2, 오른쪽 위
            vh.AddVert(new Vector3(startX, startY + height), color, new Vector2(1f, 0f)); // Vertex 3, 왼쪽 위

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        private void PopulateOutlineRect(VertexHelper vh, float width, float height, float outerX, float outerY, float innerX, float innerY)
        {
            // 외부 사각형의 버텍스 추가, Vertex 0~3
            vh.AddVert(new Vector3(outerX, outerY), color, new Vector2(0f, 0f)); // Vertex 0, 왼쪽 아래
            vh.AddVert(new Vector3(outerX + width, outerY), color, new Vector2(0f, 1f)); // Vertex 1, 오른쪽 아래
            vh.AddVert(new Vector3(outerX + width, outerY + height), color, new Vector2(1f, 1f)); // Vertex 2, 오른쪽 위
            vh.AddVert(new Vector3(outerX, outerY + height), color, new Vector2(1f, 0f)); // Vertex 3, 왼쪽 위

            // 내부 사각형의 버텍스 추가, Vertex 4~7
            // 이미 Thickness만큼 이동했으므로 최종적으로 2번 빼줘야 한다.
            vh.AddVert(new Vector3(innerX, innerY), color, new Vector2(0f, 0f)); // Vertex 4, 왼쪽 아래
            vh.AddVert(new Vector3(innerX + width - 2 * Thickness, innerY), color, new Vector2(0f, 1f)); // Vertex 5, 오른쪽 아래
            vh.AddVert(new Vector3(innerX + width - 2 * Thickness, innerY + height - 2 * Thickness), color, new Vector2(1f, 1f)); // Vertex 6, 오른쪽 위
            vh.AddVert(new Vector3(innerX, innerY + height - 2 * Thickness), color, new Vector2(1f, 0f)); // Vertex 7, 왼쪽 위

            // 사각형 Vertex Inex
            // 3 ----- 2
            // |7-----6|
            // ||     ||
            // |4-----5|
            // 0 ------1
            // 하단 마름모 그리기
            vh.AddTriangle(0, 1, 4);
            vh.AddTriangle(1, 5, 4);

            // 우측 마름모 그리기
            vh.AddTriangle(1, 2, 5);
            vh.AddTriangle(2, 6, 5);

            // 상단 마름모 그리기
            vh.AddTriangle(2, 3, 6);
            vh.AddTriangle(3, 7, 6);

            // 좌측 마름모 그리기
            vh.AddTriangle(3, 0, 7);
            vh.AddTriangle(0, 4, 7);
        }
    }
}