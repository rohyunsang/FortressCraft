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
            // �̸��� ���̰� 12�� �ʰ����� Ȯ��
            if (name.Length > 12)
            {
                return false;
            }

            // �̸� �� �ʼ��� �ִ��� Ȯ���ϴ� ���Խ�
            Regex consonantRegex = new Regex(@"(?<![\u1100-\u11FF\uAC00-\uD7AF])[\u3131-\u314E|\u3165-\u3186]+(?![\u1100-\u11FF\uAC00-\uD7AF])");

            // �̸��� ���Ǵ� ���ڸ� ���ԵǾ� �ִ��� Ȯ���ϴ� ���Խ� (���ĺ�, ����, �ѱ�)
            Regex validCharRegex = new Regex(@"^[a-zA-Z0-9\uAC00-\uD7AF]+$");

            // ��ȿ�� �̸��� �ʼ��� ����� �ϰ�, ���Ǵ� ���ڸ� ���ԵǾ� ������, ���� ������ �ʰ����� �ʾƾ� �մϴ�.
            return !consonantRegex.IsMatch(name) && validCharRegex.IsMatch(name);
        }
    }
}

