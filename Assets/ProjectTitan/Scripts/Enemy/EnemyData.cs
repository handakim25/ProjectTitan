using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan
{
    // @Further Update
    // 추후에는 Model 등을 여기서 설정하고
    // Enemy Editor를 통해서 한 번에 작성할 수 있도록 할 것
    // 그리고 CSV 파일로 저장할 수 있도록 할 것
    // 데이터 디자인은 추후에 구체적으로 설계할 것
    // 외부 데이터, 에디터 데이터, 런타임 데이터를 나누어서 관리
    public struct EnemyData
    {
        public int EnemyId;
        public string EnemyName;
        public string EnemyDescription;
        public bool IsBoss;
    }
}
