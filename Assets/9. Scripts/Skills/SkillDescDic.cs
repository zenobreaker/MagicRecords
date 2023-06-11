using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDescDic : MonoBehaviour
{
    public string SetSkillDesc(Skill _skill)
    {
        string desc = "";

        switch (_skill.MyName)
        {
            case "ReinforcedMagicBullet":
                desc = "��ȭ�� ��ź�� ��Ƴ��ϴ�. �Ӽ� źȯ�� �ִٸ� �ش� �Ӽ� źȯ�� �Һ��մϴ�. \n  ĳ������ ���ݷ���<color=#E07647>" + _skill.MyDamage.ToString()
            + "</color>%�� ���ݷ��� �����ϴ�.";
                break;
            case "IceShot":
                desc = "������ ������¿� �ɸ��� ź�� �߻��մϴ�.\n���� ����� ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() +
                    "</color>%�� ���°� 2�ʰ� ��� �ٽ��ϴ�.";
                break;
            case "PlasmaRay":
                desc = "���濡 �ö�� ������ �߻��մϴ�. \nĳ������ ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ������ �����ϴ�.";
                break;
            case "SpreadShot":
                desc = "���� Ȯ��ź�� �߻��մϴ�.  \n �� ź�� ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ���ظ� �����ϴ�.";
                break;
            case "ConsecutiveShot":
                desc = "�������� ��ź�� 5ȸ �������� ����մϴ�. \n�� źȯ���� �Ӽ�ź�� �Һ��ϸ� ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ���ظ� �����ϴ�.";
                break;
            case "SpiralShot":
                desc = "�ҿ뵹�� źȯ�� �߻��մ�. \n ���� ����� ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ���ظ� ���������� �����ϴ�.";
                break;
            case "SplitBullet":
                desc = "�������� �Ŵ��� ��ź�� �߻��մϴ�. ��ź�� ���������� �ֺ��� ��ź�� �ֺ��� �����Ͽ� ���ظ� �����ϴ�.\n " +
                    "��ź - ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ����\n" +
                    " ��ź - ���ݷ���  <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ����";
                break;
            case "BulletsRain":
                desc = "������ ��ġ�� ���߿��� źȯ�� ����Ʈ���ϴ�. \n źȯ�� ������ �����ϸ� ���ݷ��� <color=#E07647>"
                    + _skill.MyDamage.ToString() + "</color>%�� ���ظ� �����ϴ�.";
                break;
            case "ExtreamBullet":
                desc = "�Ŵ��� ������ ����Ű�� źȯ�� �߻��մϴ�.  \n ���� ����� ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% �� ���ظ� �����ϴ�.";
                break;
            case "AssistWeapons":
                desc = "�ڽ��� �ֺ��� ���ݽ� ���� �����ϴ� ���� �� ���� ��ȯ�մϴ�.  \n ������ ���ݷ��� <color=#E07647>" + _skill.MyDamage.ToString() + "</color>%�� �����ϴ�.";
                break;
        }
        return desc;
    }
}
