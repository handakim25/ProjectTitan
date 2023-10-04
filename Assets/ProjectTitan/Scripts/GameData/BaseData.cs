using System.Linq;
using UnityEngine;

// Effect
// - Prefab : 어떤 정보일지
// - Duration, 생명 주기
// - 위치 : Target Transform
// 피격 이펙트
// CLip : 데이터의 기본 단위
// Effect Clip -> Effect Data가 가지고 있다. 
// Effect Data -> Excel 같은 역할
namespace Titan.Resource
{
    /// <summary>
    /// Data의 기본 클래스
    /// 공통적인 데이터를 가지고 있다.
    /// 데이터의 개수, 이름의 목록 리스트를 가져올 수 있다.
    /// </summary>
    [System.Serializable]
    public class BaseData : ScriptableObject
    {
        /// <summary>
        /// Data가 저장되는 경로
        /// </summary>
        public const string DataDirectory = "/ProjectTitan/ResourcesData/Resources/Data/";
        /// <summary>
        /// Save에 사용할 파일 경로, 절대 경로이다. Application.dataPath + DataDirectory. /로 끝난다.
        /// </summary>
        protected virtual string SaveFilePath => Application.dataPath + DataDirectory;
        /// <summary>
        /// Resource에서 사용할 파일 경로, Load하기 위해서 사용한다.
        /// </summary>

        protected virtual string ResourcePath => "";
        /// <summary>
        /// Data의 파일 이름. 확장자 포함. Save하기 위해서 사용한다.
        /// </summary>
        protected virtual string DataFileName => "";

        /// <summary>
        /// Data의 이름. 에디터에서 표시되고 enum으로 출력된다.
        /// </summary>
        public string[] names = null;
    
        public BaseData() { }
    
        public int Count => names?.Length ?? 0;
    
        /// <summary>
        /// Tool에서 사용할 이름 리스트
        /// </summary>
        /// <param name="showID">ID 포함</param>
        /// <param name="filterWord">필터할 단어</param>
        /// <returns>가지고 있는 이름 목록, 없을 경우 비어 있는 리스트 반환</returns>
        public string[] GetNameList(bool showID, string filterWord = "")
        {
            string[] retList = new string[0];
            if(names == null)
            {
                return retList;
            }
    
            retList = new string[names.Length];
            for(int i = 0; i < names.Length; i++)
            {
                if(filterWord != "")
                {
                    if(names[i].ToLower().Contains(filterWord.ToLower()) == false)
                    {
                        continue;
                    }
                }
                
                if(showID)
                {
                    retList[i] = $"{i.ToString()} : {names[i]}";
                }
                else
                {
                    retList[i] = names[i];
                }
            }
            return retList;
        }
    
        // @Think
        // 순수 가상 함수로 만드는 것은 어때?
        /// <summary>
        /// 해당 이름을 가진 빈 데이터를 생성한다.
        /// </summary>
        /// <param name="newName">새로운 데이터 이름</param>
        /// <returns>현재 데이터 개수</returns>
        public virtual int AddData(string newName)
        {
            return Count;
        }
        
        public virtual void RemoveData(int index)
        {
    
        }
    
        public virtual void Copy(int index)
        {
    
        }

        public virtual void ClearData()
        {
            
        }

        public virtual void SaveData()
        {

        }

        public virtual void LoadData()
        {

        }
    }
}