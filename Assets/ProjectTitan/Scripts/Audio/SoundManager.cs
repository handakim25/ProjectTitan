using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Titan.Utility;
using Titan.Core;

namespace Titan.Audio
{
    sealed public class SoundManager : MonoSingleton<SoundManager>
    {
        // Audio Mixer
        private const string MixerName = "AudioMixer";
        private const string MasterGroupName = "Master";
        private const string BGMGroupName = "BGM";
        private const string EffectGroupName = "Effect";
        private const string UIGroupName = "UI";
        private const string MasterVolumeParam = "MasterVolume";
        private const string BGMVolumeParam = "BGMVolume";
        private const string EffectVolumeParam = "EffectVolume";
        private const string UIVolumeParam = "UIVolume";

        private const string ContainerName = "SoundContainer";
        private const string BGMSourceNameA = "BGMSourceA";
        private const string BGMSourceNameB = "BGMSourceB";
        private const string EffectSourceName = "EffectSource";
        private const string UISourceName = "UISource";

        public enum BGMPlayingType
        {
            None = 0,
            SourceA = 1,
            SourceB = 2,
            AtoB = 3,
            BtoA = 4,
        }

        public enum AudioType
        {
            Master,
            BGM,
            Effect,
            UI,
        }

        private AudioMixer _mixer;
        // Volume : -80~0
        public const float MinVolume = -80.0f;
        public const float MaxVolume = 0f;

      private Transform _audioRoot;
        private AudioSource _bgmSourceA;
        private AudioSource _bgmSourceB;
        [SerializeField] private int _effectChanelCount = 5;
        private AudioSource[] _effectSources;
        private float[] _effectPlayStartTime;
        private AudioSource _uiSource;

        // 이거 꺼질 때 같이 꺼져야 되지 않나?
        private bool _isTicking = false;

        // Fade in / Fade out
        private BGMPlayingType _currentPlayingType = BGMPlayingType.None;
        private SoundClip _currentSound = null;
        private SoundClip _lastSound = null;

        #region Unity Methods
        
        private void Start()
        {
            if(_mixer == null)
            {
                _mixer = Resources.Load(MixerName) as AudioMixer;
            }
            if(_audioRoot == null)
            {
                _audioRoot = new GameObject(ContainerName).transform;
                _audioRoot.SetParent(transform);
            }

            _bgmSourceA = CreateAudioSource(BGMSourceNameA);
            _bgmSourceB = CreateAudioSource(BGMSourceNameB);
            _uiSource = CreateAudioSource(UISourceName);

            _effectPlayStartTime = new float[_effectChanelCount];
            _effectSources = new AudioSource[_effectChanelCount];
            for(int i = 0; i < _effectChanelCount; i++)
            {
                _effectPlayStartTime[i] = 0.0f;
                _effectSources[i] = CreateAudioSource($"{EffectSourceName}_{i}");
            }

            if(_mixer != null)
            {
                _bgmSourceA.outputAudioMixerGroup = _mixer.FindMatchingGroups(BGMGroupName)[0];
                _bgmSourceB.outputAudioMixerGroup = _mixer.FindMatchingGroups(BGMGroupName)[0];
                _uiSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(UIGroupName)[0];
                foreach(var audio in _effectSources)
                {
                    audio.outputAudioMixerGroup = _mixer.FindMatchingGroups(EffectGroupName)[0];
                }
            }

            InitVolume();
        }

        // Fade In / Out 처리
        private void Update()
        {
            if(_currentSound == null)
            {
                return;
            }

            // Process Fade
            if(_currentPlayingType == BGMPlayingType.SourceA)
            {
                // Check Process와도 비슷하게 진행되는 측면이 있어
                // 서로를 좀 잘 매칭시켜줄 방법이 없을까?
                // current와 last가 포인터를 가지고 있다면 해결할 수는 있는데
                // 복잡도가 음...
                // 렙핑을 한다면?
                _currentSound.DoFade(Time.deltaTime, _bgmSourceA);
            }
            else if(_currentPlayingType == BGMPlayingType.SourceB)
            {
                _currentSound.DoFade(Time.deltaTime, _bgmSourceB);
            }
            else if(_currentPlayingType == BGMPlayingType.AtoB)
            {
                _lastSound.DoFade(Time.deltaTime, _bgmSourceA);
                _currentSound.DoFade(Time.deltaTime, _bgmSourceB);
            }
            else if(_currentPlayingType == BGMPlayingType.BtoA)
            {
                _lastSound.DoFade(Time.deltaTime, _bgmSourceB);
                _currentSound.DoFade(Time.deltaTime, _bgmSourceA);
            }

            // Check whether fade end
            if(_bgmSourceA.isPlaying && _bgmSourceB.isPlaying == false)
            {
                _currentPlayingType = BGMPlayingType.SourceA;
            }
            else if(_bgmSourceB.isPlaying && _bgmSourceA.isPlaying == false)
            {
                _currentPlayingType = BGMPlayingType.SourceB;
            }
            else if(_bgmSourceA.isPlaying == false && _bgmSourceB.isPlaying == false)
            {
                _currentPlayingType = BGMPlayingType.None;
            }
        }
        
        #endregion Unity Methods

        private AudioSource CreateAudioSource(string sourceName)
        {
            AudioSource retAudio = new GameObject(sourceName, typeof(AudioSource)).GetComponent<AudioSource>();
            retAudio.transform.SetParent(_audioRoot);
            retAudio.playOnAwake = false;
            return retAudio;
        }

        #region Audio Mixer
        
        /// <summary>
        /// Set Volume, UI로부터 입력이 들어온다
        /// </summary>
        /// <param name="type">설정할 오디오의 타입</param>
        /// <param name="currnetRatio">0~1</param>
        /// <returns>Mixer Volume : -80~0 </returns>
        public float SetVolume(AudioType type, float currnetRatio)
        {
            currnetRatio = Mathf.Clamp(currnetRatio, 0.0001f, 1f);
            float volume = Mathf.Log10(currnetRatio) * 20f;
            _mixer.SetFloat(GetVolumeParam(type), volume);
            return volume;
        }

        /// <summary>
        /// 저장되어 있는 볼륨 값을 읽어온다. 기본값은 MaxVolume으로 지정되어 있다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>0~1</returns>
        public float GetVolume(AudioType type)
        {
            float volume = 0.0f;
            if(PlayerPrefs.HasKey(GetVolumeParam(type)))
            {
                // PlayerPrefs Volume : -80~0 Mixer Volume
                volume = PlayerPrefs.GetFloat(GetVolumeParam(type));
            }
            else
            {
                volume = MaxVolume;
            }
            // 
            return Mathf.Pow(10, volume / 20);
        }

        private string GetVolumeParam(AudioType type)
        {
            return type switch
            {
                AudioType.Master => MasterVolumeParam,
                AudioType.BGM => BGMVolumeParam,
                AudioType.Effect => EffectVolumeParam,
                AudioType.UI => EffectVolumeParam,
                _ => MasterVolumeParam,
            };
        }

        private void InitVolume()
        {
            if(_mixer != null)
            {
                _mixer.SetFloat(MasterVolumeParam, Mathf.Log10(GetVolume(AudioType.Master)) * 20f);
                _mixer.SetFloat(BGMVolumeParam, Mathf.Log10(GetVolume(AudioType.Master)) * 20f);
                _mixer.SetFloat(EffectVolumeParam, Mathf.Log10(GetVolume(AudioType.Master)) * 20f);
                _mixer.SetFloat(UIVolumeParam, Mathf.Log10(GetVolume(AudioType.Master)) * 20f);
            }
        }
        
        #endregion Audio Mixer

        #region Basic Audio
        
        // 가장 기본이 되는 재생
        private void PlayAudioSource(AudioSource source, SoundClip clip, float volume)
        {
            if(source == null || clip == null)
            {
                return;
            }

            source.Stop();
            source.clip = clip.GetClip();
            source.volume = volume;
            source.loop = clip.IsLoop;
            source.pitch = clip.pitch;
            source.dopplerLevel = clip.dopplerLevel;
            source.rolloffMode = clip.rolloffMode;
            source.minDistance = clip.minDistance;
            source.maxDistance = clip.maxDistance;
            source.spatialBlend = clip.spatialBlend;
            source.Play();
        }

        private void PlayAudioSource(AudioSource source, SoundClip clip)
        {
            if(source == null || clip == null)
            {
                return;
            }
            PlayAudioSource(source, clip, clip.maxVolume);
        }

        private void PlayAudioSourceAtPoint(SoundClip clip, Vector3 position, float volume)
        {
            if(clip == null)
            {
                return;
            }
            AudioSource.PlayClipAtPoint(clip.GetClip(), position, volume);
        }

        private void PlayAudioSourceAtPoint(SoundClip clip, Vector3 position)
        {
            if(clip == null)
            {
                return;
            }
            PlayAudioSourceAtPoint(clip, position, clip.maxVolume);
        }
        
        #endregion Basic Audio

        #region BGM
        
        public bool IsPlayingBGM => _currentPlayingType != BGMPlayingType.None;

        /// <summary>
        /// 동일한 BGM 재생인지를 확인
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public bool IsDifferentSound(SoundClip clip)
        {
            if(clip == null)
            {
                return false;
            }

            if(_currentSound != null && _currentSound.index == clip.index 
                && IsPlayingBGM && _currentSound.isFadeOut == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Start Check Loop Coroutine
        // 이렇게 구현하면 여러군데에서 실행해서 중복 실행되지 않나?
        public void StartCheckLoop()
        {
            StartCoroutine(CheckProcess());
        }        

        // Check Loop를 확인하는 코루틴
        // Loop를 가지고 있으면 Loop를 체크한다.
        private IEnumerator CheckProcess()
        {
            var wait = new WaitForSeconds(0.05f);
            while(_isTicking && IsPlayingBGM)
            {
                yield return wait;
                if(_currentSound.HasLoop)
                {
                    // Think.
                    // 어차피 CheckLoop에서 HasLoop를 처리하는데 굳이 이렇게 할 필요가 있나?
                    // 그냥 아예 다 호출해도 될 거 같은데
                    if(_currentPlayingType == BGMPlayingType.SourceA)
                    {
                        // @Caution : Current Sound가 갑자기 null이 되지 않도록 주의할 것
                        _currentSound.CheckLoop(_bgmSourceA);
                    }
                    else if(_currentPlayingType == BGMPlayingType.SourceB)
                    {
                        _currentSound.CheckLoop(_bgmSourceB);
                    }
                    else if(_currentPlayingType == BGMPlayingType.AtoB)
                    {
                        _lastSound.CheckLoop(_bgmSourceA);
                        _currentSound.CheckLoop(_bgmSourceB);
                    }
                    else if(_currentPlayingType == BGMPlayingType.BtoA)
                    {
                        _lastSound.CheckLoop(_bgmSourceB);
                        _currentSound.CheckLoop(_bgmSourceA);
                    }
                }
            }
        }

        // Fade In, 새로운 소리가 밖에서 안으로 들어온다
        // 근데 이거 자연스럽게 Fade 겹치게 하는게 낫지 않나?
        public void FadeIn(SoundClip clip, float time, Interpolate.EaseType ease)
        {
            if(!IsDifferentSound(clip))
            {
                return;
            }

            _bgmSourceA.Stop();
            _bgmSourceB.Stop();
            _lastSound = _currentSound;
            _currentSound = clip;
            PlayAudioSource(_bgmSourceA, _currentSound, 0.0f);
            _currentSound.FadeIn(time, ease); // Fade 설정
            _currentPlayingType = BGMPlayingType.SourceA;
            if(_currentSound.HasLoop)
            {
                _isTicking = true;
                StartCheckLoop();
            }
        }

        public void FadeIn(int index, float time, Interpolate.EaseType ease)
        {
            FadeIn(DataManager.SoundData.GetCopy(index), time, ease);
        }

        public void FadeOut(float time, Interpolate.EaseType ease)
        {
            if(_currentSound != null)
            {
                _currentSound.FadeOut(time, ease);
            }
        }

        public void FadeTo(SoundClip clip, float time, Interpolate.EaseType ease)
        {
            // 재생 중인 음악이 없을 경우 Fade In
            if(_currentPlayingType == BGMPlayingType.None)
            {
                FadeIn(clip, time, ease);
                return; 
            }

            if(IsDifferentSound(clip) == false)
            {
                return;
            }

            // 진행 중인 Fade를 정지
            if(_currentPlayingType == BGMPlayingType.AtoB)
            {
                _bgmSourceA.Stop();
                _currentPlayingType = BGMPlayingType.SourceB;
            }
            else if(_currentPlayingType == BGMPlayingType.BtoA)
            {
                _bgmSourceB.Stop();
                _currentPlayingType = BGMPlayingType.SourceA;
            }
            _lastSound = _currentSound;
            _currentSound = clip;
            _lastSound.FadeOut(time, ease);
            _currentSound.FadeIn(time, ease);

            if(_currentPlayingType == BGMPlayingType.SourceA)
            {
                PlayAudioSource(_bgmSourceB, _currentSound, 0.0f);
                _currentPlayingType = BGMPlayingType.AtoB;
            }
            else if(_currentPlayingType == BGMPlayingType.SourceB)
            {
                PlayAudioSource(_bgmSourceA, _currentSound, 0.0f);
                _currentPlayingType = BGMPlayingType.BtoA;
            }

            if(_currentSound.HasLoop)
            {
                _isTicking = true;
                StartCheckLoop();
            }
        }

        public void FadeTo(int index, float time, Interpolate.EaseType ease)
        {
            // Copy하는 것이 맞을까? unload 정책도 구현해야할 지도 몰라
            FadeTo(DataManager.SoundData.GetCopy(index), time, ease);
        }

        #endregion BGM

        public void PlayBGM(SoundClip clip)
        {
            if(IsDifferentSound(clip))
            {
                _bgmSourceB.Stop();
                _lastSound = _currentSound;
                _currentSound = clip;
                PlayAudioSource(_bgmSourceA, clip, clip.maxVolume);
                if(_currentSound.HasLoop)
                {
                    _isTicking = true;
                    StartCheckLoop();
                }
            }
        }

        public void PlayBGM(int index)
        {
            SoundClip clip = DataManager.SoundData.GetCopy(index);
            PlayBGM(clip);
        }

        public void PlayEffectSound(SoundClip clip)
        {
            bool isPlaySuccess = false;
            for(int i = 0; i < _effectSources.Length; i++)
            {
                if(_effectSources[i].isPlaying == false)
                {
                    PlayAudioSource(_effectSources[i], clip);
                    _effectPlayStartTime[i] = Time.realtimeSinceStartup;
                    isPlaySuccess = true;
                    break;
                }
                else if(_effectSources[i].clip == clip?.GetClip())
                {
                    _effectSources[i].Stop();
                    PlayAudioSource(_effectSources[i], clip);
                    _effectPlayStartTime[i] = Time.realtimeSinceStartup;
                    isPlaySuccess = true;
                    break;
                }
            }

            if(isPlaySuccess == false)
            {
                float maxTime = 0f;
                int selectIndex = 0;
                for(int i = 0; i < _effectSources.Length; i++)
                {
                    if(_effectPlayStartTime[i] > maxTime)
                    {
                        maxTime = _effectPlayStartTime[i];
                        selectIndex = i;
                    }
                }
                PlayAudioSource(_effectSources[selectIndex], clip);
                _effectPlayStartTime[selectIndex] = Time.realtimeSinceStartup;
            }
        }

        public void PlayEffectSound(int index)
        {
            SoundClip clip = DataManager.SoundData.GetCopy(index);
            PlayEffectSound(clip);
        }

        public void PlayEffectSound(int index, Vector3 position)
        {
            SoundClip clip = DataManager.SoundData.GetCopy(index);
            PlayEffectSound(clip, position);
        }

        public void PlayEffectSound(SoundClip clip, Vector3 position)
        {
            PlayAudioSourceAtPoint(clip, position);
        }

        public void PlayOneShotEffect(int index, Vector3 position, float volume)
        {
            SoundClip clip = DataManager.SoundData.GetCopy(index);
            if(clip == null)
            {
                return;
            }
            PlayEffectSound(clip, position);
        }

        public void PlayOneShot(SoundClip clip)
        {
            if(clip == null)
            {
                return;
            }

            switch(clip.playType)
            {
                case SoundPlayType.BGM:
                    PlayBGM(clip);
                    break;
                case SoundPlayType.EFFECT:
                    PlayEffectSound(clip);
                    break;
                case SoundPlayType.UI:
                    PlayUISound(clip);
                    break;
            }
        }

        public void PlayUISound(SoundClip clip)
        {
            if(clip == null)
            {
                return;
            }
            PlayAudioSource(_uiSource, clip, clip.maxVolume);
        }

        public void PlayUISound(int index)
        {
            SoundClip clip = DataManager.SoundData.GetCopy(index);
            PlayUISound(clip);
        }

        public void Stop(bool allStop = false)
        {
            if(allStop)
            {
                _bgmSourceA.Stop();
                _bgmSourceB.Stop();
            }

            FadeOut(0.5f, Interpolate.EaseType.Linear);
            _currentPlayingType = BGMPlayingType.None;
            StopAllCoroutines();
        }
    }
}
