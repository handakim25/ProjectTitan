using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Utility;
using Titan.Resource;

namespace Titan.Audio
{
    /// <summary>
    /// Sound 관련 데이터
    /// </summary>
    public class SoundClip
    {
        public int index = 0;
        public string clipName = string.Empty;
        public string clipPath = string.Empty;
        public string clipFullPath = string.Empty;
        public SoundPlayType playType = SoundPlayType.None;

        // Read only Setting
        // @Refactor
        // 함수를 이용해서 설정하고 property로 접근?
        private AudioClip clip = null;
        public float maxVolume = 1.0f;
        public bool IsLoop = false; // This Audio source is loop
        public float pitch = 1.0f;
        public float dopplerLevel = 1.0f;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        public float minDistance = 10000.0f;
        public float maxDistance = 50000.0f;
        public float spartialBlend = 1.0f;

        // Loop Feature
        public float[] setTime = new float[0]; // Audio Loop start time
        public float[] checkTime = new float[0]; // Audio Loop end time
        public int curLoopIndex = 0;

        // Fade Feature
        public bool isFadeIn = false;
        public bool isFadeOut = false;
        public float fadeTimer = 0.0f;
        public float fadeDuration = 0.0f;
        public Interpolate.EaseType easeType = Interpolate.EaseType.Linear;
        private Interpolate.InterpolateFunc interpolateFunc;

        public SoundClip() {}
        public SoundClip(string clipPath, string clipName)
        {
            this.clipName = clipName;
            this.clipPath = clipPath;
        }

        #region Resource
        
        public bool PreLoad()
        {
            if(clip != null)
            {
                return true;
            }
            clipFullPath = clipPath + clipName;
            if(clipFullPath == string.Empty)
            {
                return false;
            }

            clip = ResourceManager.Load(clipPath + clipName) as AudioClip;
            return clip != null; 
        }
        public AudioClip GetClip()
        {
            if(clip == null && PreLoad() == false)
            {
                Debug.LogWarning($"Can not load Audio clip resource {clipName}");
                return null;
            }

            return clip;
        }
        
        #endregion Resource
        
        #region Loop
        
        public void AddLoop()
        {
            // 단점 : checkTime의 형식을 알아야만 작성이 가능하다
            checkTime = checkTime.Concat(new [] {0.0f}).ToArray();
            setTime = setTime.Concat(new [] {0.0f}).ToArray();
        }
        
        public void RemoveLoop(int removeIndex)
        {
            checkTime = checkTime.Where((name, index) => index != removeIndex).ToArray();
            setTime = setTime.Where((name, index) => index != removeIndex).ToArray();
        }

        public bool HasLoop => checkTime.Length > 0;

        public void NextLoop()
        {
            curLoopIndex++;
            if(curLoopIndex >= checkTime.Length)
            {
                curLoopIndex = 0;
            }
        }

        public void CheckLoop(AudioSource source)
        {
            if(HasLoop && source.time >= checkTime[curLoopIndex])
            {
                source.time = setTime[curLoopIndex];
                // NextLoop(); // ?
            }
        }
        
        #endregion Loop

        #region Fade
        
        public void FadeIn(float duration, Interpolate.EaseType easeType)
        {
            isFadeOut = false;
            fadeTimer = 0.0f;
            fadeDuration = duration;
            interpolateFunc = Interpolate.Ease(easeType);
            isFadeIn = true;
        }

        public void FadeOut(float duration, Interpolate.EaseType easeType)
        {
            isFadeIn = false;
            fadeTimer = 0.0f;
            fadeDuration = duration;
            interpolateFunc = Interpolate.Ease(easeType);
            isFadeOut = true;
        }

        public void DoFade(float time, AudioSource source)
        {
            if(isFadeIn)
            {
                fadeTimer += time;
                source.volume = Interpolate.Ease(interpolateFunc, 0, maxVolume, fadeTimer, fadeDuration);
                if(fadeTimer >= fadeDuration)
                {
                    isFadeIn = false;
                }
            }
            else if(isFadeOut)
            {
                fadeTimer += time;
                source.volume = Interpolate.Ease(interpolateFunc, maxVolume, 0 - maxVolume, fadeTimer, fadeDuration);
                if(fadeTimer >= fadeDuration)
                {
                    isFadeOut = false;
                    source.Stop();
                }
            }
        }
        
        #endregion Fade
    }

    public enum SoundPlayType
    {
        None = -1,
    }
}