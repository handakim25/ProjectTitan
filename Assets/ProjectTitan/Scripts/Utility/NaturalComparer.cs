using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Titan
{
    // Natural Sort
    // 1. https://blog.codinghorror.com/sorting-for-humans-natural-sort-order/
    // 2. https://stackoverflow.com/questions/1022203/sorting-strings-containing-numbers-in-a-user-friendly-way
    // 일반적으로 구현할려면 굉장히 복잡해서 그나마 정규식 관련 함수를 찾음
    // StrCmpLogicalW 함수는 Windows에 의존적인 함수라서 사용을 하지 않기로 결정
    public class NaturalComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return NaturalCompare(x, y);
        }

        public int NaturalCompare(string x, string y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            // 문자열을 숫자와 문자열로 나눈다.
            // \D는 숫자가 아닌 문자, \d는 숫자
            // (?<=\D)(?=\d) : 숫자 앞에 문자가 있는 경우
            // (?<=\d)(?=\D) : 문자 앞에 숫자가 있는 경우
            var regex = new Regex(@"(?<=\D)(?=\d)|(?<=\d)(?=\D)");
            var tokenX = regex.Split(x);
            var tokenY = regex.Split(y);

            for (int i = 0; i < Mathf.Min(tokenX.Length, tokenY.Length); i++)
            {
                // 둘다 숫자일 경우
                if (long.TryParse(tokenX[i], out long resultX) && long.TryParse(tokenY[i], out long resultY))
                {
                    if (resultX != resultY)
                    {
                        return resultX.CompareTo(resultY);
                    }
                }
                // 일반 비교, 만약에 동일할 경우 다음 토큰으로 넘어간다.
                else
                {
                    int stringCompare = string.Compare(tokenX[i], tokenY[i], System.StringComparison.OrdinalIgnoreCase);
                    if (stringCompare != 0)
                    {
                        return stringCompare;
                    }
                }
            }

            // 둘 중 하나가 끝났다면
            return tokenX.Length.CompareTo(tokenY.Length);
        }
    }
}
