using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Effects;
using Titan.Audio;
using Titan.QuestSystem;
using Titan.GameEventSystem;
using Titan.InventorySystem.Items;

namespace Titan
{
    /// <summary>
    /// Data들을 가지고 있는 오브젝트, Start에서 데이터를 로드하므로 초기화 과정에서 데이터를 참조하지 않도록 주의
    /// 또한, 게임 진행은 초기화를 거치도록 한다.
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static EffectData _effectData = null;
        private static SoundData _soundData = null;
        private static QuestObjectDatabase _questDatabase = null;
        private static GameEventData _gameEventData = null;
        [SerializeField] private ItemDatabaseObject _itemDatabase = null;
        private static ItemDatabase _itemDatabaseStatic = null;

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
            if(_itemDatabase != null)
            {
                _itemDatabaseStatic = ScriptableObject.CreateInstance<ItemDatabase>();
                _itemDatabaseStatic.itemDatabase = _itemDatabase;
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

        public static ItemDatabase ItemDatabase
        {
            get
            {
                return _itemDatabaseStatic;
            }
        }
    }
}
