using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Core
{
   public enum GameStatus
   {
        None, // 게임 시작 전 메인 화면
        Pause, // UI를 활성화 했을 경우
        Play, // 게임 진행 중
   }
}
