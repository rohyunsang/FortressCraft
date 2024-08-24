using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Agit.FortressCraft // Util
{
    public static class PlayerNameValidator
    {
        public static bool IsValidName(string name)
        {
            // 이름의 길이가 12자 초과인지 확인
            if (name.Length > 12)
            {
                return false;
            }

            // 이름 중 초성만 있는지 확인하는 정규식
            Regex consonantRegex = new Regex(@"(?<![\u1100-\u11FF\uAC00-\uD7AF])[\u3131-\u314E|\u3165-\u3186]+(?![\u1100-\u11FF\uAC00-\uD7AF])");

            // 이름에 허용되는 문자만 포함되어 있는지 확인하는 정규식 (알파벳, 숫자, 한글)
            Regex validCharRegex = new Regex(@"^[a-zA-Z0-9\uAC00-\uD7AF]+$");

            // 유효한 이름은 초성만 없어야 하고, 허용되는 문자만 포함되어 있으며, 길이 제한을 초과하지 않아야 합니다.
            return !consonantRegex.IsMatch(name) && validCharRegex.IsMatch(name);
        }
    }
}

