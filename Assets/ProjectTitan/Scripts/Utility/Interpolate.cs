using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Utility
{
    // reference : https://easings.net/
    public static class Interpolate
    {
        public delegate float InterpolateFunc(float start, float distance, float elapsedTime, float duration);

        public enum EaseType
        {
            Linear,
            EaseInSine,
            EaseOutSine,
        }

        public static float Ease(InterpolateFunc ease, float start, float distance, float elapsedTime, float duration)
        {
            return ease(start, distance, elapsedTime, duration);
        }
        
        public static Vector2 Ease(InterpolateFunc ease, Vector2 start, Vector2 distance, float elapsedTime, float duration)
        {
            start.x = ease(start.x, distance.x, elapsedTime,duration);
            start.y = ease(start.y, distance.y, elapsedTime,duration);
            return start;
        }

        public static Vector3 Ease(InterpolateFunc ease, Vector3 start, Vector3 distance, float elapsedTime, float duration)
        {
            start.x = ease(start.x, distance.x, elapsedTime,duration);
            start.y = ease(start.y, distance.y, elapsedTime,duration);
            start.z = ease(start.z, distance.z, elapsedTime,duration);
            return start;
        }

        // Type에 맞는 Interplate Func Delegate를 반환
        public static InterpolateFunc GetEase(EaseType type)
        {
            return type switch
            {
                EaseType.Linear => new InterpolateFunc(Interpolate.Linear),
                EaseType.EaseInSine => new InterpolateFunc(Interpolate.EaseInSine),
                EaseType.EaseOutSine => new InterpolateFunc(Interpolate.EaseOutSine),
                _ => new InterpolateFunc(Interpolate.Linear),
            };
        }

        public static float Linear(float start, float distance, float elapsedTime, float duration)
        {
            if(elapsedTime > duration) { elapsedTime = duration;}
            return distance * (elapsedTime / duration) + start;
        }

        public static float EaseInSine(float start, float distance, float elapsedTime, float duration)
        {
            if(elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            // 1 - cos(x / 2), x : progress
            // distance(1 - cos(progress / 2))
            return start + distance - distance * Mathf.Cos((elapsedTime / duration) * (Mathf.PI / 2));
        }

        public static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
        {
            if(elapsedTime > duration)
            {
                elapsedTime = duration;
            }
            // sin(x / 2)
            return start + distance * Mathf.Sin((elapsedTime / duration) * (Mathf.PI / 2));
        }
    }
}
