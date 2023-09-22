using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Effects;
using Titan.Audio;
using Titan.QuestSystem;
using Titan.GameEventSystem;

namespace Titan
{
    /// <summary>
    /// Data들을 가지고 있는 오브젝트
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static EffectData _effectData = null;
        private static SoundData _soundData = null;
        private static QuestObjectDatabase _questDatabase = null;
        private static GameEventData _gameEventData = null;

        private void Start()
        {
            if(_effectData == null)
            {
                _effectData = ScriptableObject.CreateInstance<EffectData>();
                _effectData.LoadData();
            }
            if(_soundData == null)
            {
                _soundData = ScriptableObject.CreateInstance<SoundData>();
                _soundData.LoadData();
            }
            if(_questDatabase == null)
            {
                _questDatabase = ScriptableObject.CreateInstance<QuestObjectDatabase>();
                _questDatabase.LoadFromJson();
            }
            if(_gameEventData == null)
            {
                _gameEventData = ScriptableObject.CreateInstance<GameEventData>();
                _gameEventData.LoadData();
            }
        }

        public static EffectData EffectData
        {
            get
            {
                if(_effectData == null)
                {
                    _effectData = ScriptableObject.CreateInstance<EffectData>();
                    _effectData.LoadData();
                }
                return _effectData;
            }
        }

        public static SoundData SoundData
        {
            get
            {
                if(_soundData == null)
                {
                    _soundData = ScriptableObject.CreateInstance<SoundData>();
                    _soundData.LoadData();
                }
                return _soundData;
            }
        }

        public static QuestObjectDatabase QuestDatabase
        {
            get
            {
                if(_questDatabase == null)
                {
                    _questDatabase = ScriptableObject.CreateInstance<QuestObjectDatabase>();
                    _questDatabase.LoadFromJson();
                }
                return _questDatabase;
            }
        }

        public static GameEventData GameEventData
        {
            get
            {
                if(_gameEventData == null)
                {
                    _gameEventData = ScriptableObject.CreateInstance<GameEventData>();
                    _gameEventData.LoadData();
                }
                return _gameEventData;
            }
        }
    }
}
