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
                desc = "강화된 마탄을 쏘아냅니다. 속성 탄환이 있다면 해당 속성 탄환을 소비합니다. \n  캐릭터의 공격력의<color=#E07647>" + _skill.MyDamage.ToString()
            + "</color>%의 공격력을 갖습니다.";
                break;
            case "IceShot":
                desc = "맞으면 빙결상태에 걸리는 탄을 발사합니다.\n맞은 대상은 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() +
                    "</color>%의 위력과 2초간 얼어 붙습니다.";
                break;
            case "PlasmaRay":
                desc = "전방에 플라즈마 광선을 발사합니다. \n캐릭터의 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 위력을 갖습니다.";
                break;
            case "SpreadShot":
                desc = "넓은 확산탄을 발사합니다.  \n 각 탄당 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해를 입힙니다.";
                break;
            case "ConsecutiveShot":
                desc = "전방으로 마탄을 5회 연속으로 사격합니다. \n각 탄환별로 속성탄을 소비하며 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해를 입힙니다.";
                break;
            case "SpiralShot":
                desc = "소용돌이 탄환을 발사합다. \n 닿은 대상은 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해를 지속적으로 입힙니다.";
                break;
            case "SplitBullet":
                desc = "전방으로 거대한 모탄을 발사합니다. 모탄은 지속적으로 주변에 자탄을 주변에 사출하여 피해를 입힙니다.\n " +
                    "모탄 - 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해\n" +
                    " 자탄 - 공격력의  <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해";
                break;
            case "BulletsRain":
                desc = "지정된 위치에 공중에서 탄환을 떨어트립니다. \n 탄환은 닿으면 폭발하며 공격력의 <color=#E07647>"
                    + _skill.MyDamage.ToString() + "</color>%의 피해를 입힙니다.";
                break;
            case "ExtreamBullet":
                desc = "거대한 폭발을 일으키는 탄환을 발사합니다.  \n 닿은 대상은 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>% 의 피해를 입힙니다.";
                break;
            case "AssistWeapons":
                desc = "자신의 주변에 공격시 같이 공격하는 포신 두 개를 소환합니다.  \n 포신은 공격력의 <color=#E07647>" + _skill.MyDamage.ToString() + "</color>%을 갖습니다.";
                break;
        }
        return desc;
    }
}
