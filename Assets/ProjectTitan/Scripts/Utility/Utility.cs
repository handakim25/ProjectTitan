using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    public static class ColorExtentions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g,color.b, alpha);
        }
    }
}
