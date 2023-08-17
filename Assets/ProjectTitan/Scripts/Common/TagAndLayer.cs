using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public static class TagAndLayer
    {
        public class LayerName
        {
            public const string Default =  "Default";
            public const string TransparentFX =  "TransparentFX";
            public const string IgnoreRaycast =  "IgnoreRaycast";
            public const string Water =  "Water";
            public const string UI =  "UI";
            public const string Ground =  "Ground"; // 지상 판정을 체크하기 위함
            public const string Interactable =  "Interactable"; // 상호 작용 오브젝트 검색
            public const string Player = "Player"; // Enemy가 Player 검색
            public const string Enemy = "Enemy"; // Player가 Enemy 검색
            public const string Destructable = "Destructable"; // 파괴 가능 물체. Enemy와 나눌 필요가 있나?
            public const string Minimap = "Minimap"; // Minimap Objects. Minimap Camera가 culling을 하기 위해 사용
        }

        public enum LayerIndex
        {
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Water = 4,
            UI = 5,
            Ground = 8,
            Interactable = 9,
            Player = 10,
            Enemy = 11,
            Destructable = 12,
            Minimap = 13,
        }

        public static class LayerMasking
        {
            public const int Default = 1 << 0;
            public const int TransparentFX = 1 << 1;
            public const int IgnoreRaycast = 1 << 2;
            public const int Water = 1 << 4;
            public const int UI = 1 << 5;
            public const int Ground = 1 << 8;
            public const int Interactable = 1 << 9;
            public const int Player = 1 << 10;
            public const int Enemy = 1 << 11;
            public const int Destructable = 1 << 12;
            public const int Minimap = 1 << 13;
        }
    }
}
