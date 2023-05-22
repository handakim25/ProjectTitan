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
            public const string Ground =  "Ground";
            public const string Interactable =  "Interactable";
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
        }
    }
}
