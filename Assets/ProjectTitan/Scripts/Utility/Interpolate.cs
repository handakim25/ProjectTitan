using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Utility
{
    public static class Interpolate
    {
        public delegate float InterpolateFunc(float start, float distance, float elapsedTime, float duration);

        public enum EaseType
        {
            Linear,
        }

        public static float Ease(InterpolateFunc ease, float start, float distance, float elapsedTime, float duration)
        {
            return ease(start, distance, elapsedTime, duration);
        }

        // Type에 맞는 Interplate Func Delegate를 반환
        public static InterpolateFunc Ease(EaseType type)
        {
            return type switch
            {
                EaseType.Linear => new InterpolateFunc(Interpolate.Linear),
                _ => new InterpolateFunc(Interpolate.Linear),
            };
        }

        public static float Linear(float start, float distance, float elapsedTime, float duration)
        {
            if(elapsedTime > duration) { elapsedTime = duration;}
            return distance * (elapsedTime / duration) + start;
        }
    }
}
