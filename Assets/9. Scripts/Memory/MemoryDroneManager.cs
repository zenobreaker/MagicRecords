using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryDroneManager : MonoBehaviour
{

    public static MemoryDroneManager instance;


    // 메모리드론 매니저는 세트효과를 위해 기록한다 여기서 거의 모든 효과를 기록하고 
    // 장착한 메모리 착용하는 것에 따라 메모리 효과를 발생시킬 수 있다
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // 대상 캐릭터에게서 장착한 드론 정보를 가져온다 
    
    // 장착한 드론의 세트효과가 동일한 개수를 체크 

    // 효과를 발생시킨다. 
    // 효과에 관련된 변수 (enum같은 것)를 체크해서 드론에 장착된 고유 변수를 확인 후 해당 변수가 가진 옵션을 여기서 발동
    // 무슨 세트를 만들까?
    // 1. 지혜 (크리티컬) 
    // 2. 탐구 (마나 및 마나 회복량)
    // 3. 용기 (공격력)
    // 4. 근성 (체력 및 체력 회복량)
    // 5. ?? (흡혈)  
    // 6. ??? (방어력 관련) 탐구 

}
