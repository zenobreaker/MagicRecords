using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryDroneManager : MonoBehaviour
{

    public static MemoryDroneManager instance;


    // �޸𸮵�� �Ŵ����� ��Ʈȿ���� ���� ����Ѵ� ���⼭ ���� ��� ȿ���� ����ϰ� 
    // ������ �޸� �����ϴ� �Ϳ� ���� �޸� ȿ���� �߻���ų �� �ִ�
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // ��� ĳ���Ϳ��Լ� ������ ��� ������ �����´� 
    
    // ������ ����� ��Ʈȿ���� ������ ������ üũ 

    // ȿ���� �߻���Ų��. 
    // ȿ���� ���õ� ���� (enum���� ��)�� üũ�ؼ� ��п� ������ ���� ������ Ȯ�� �� �ش� ������ ���� �ɼ��� ���⼭ �ߵ�
    // ���� ��Ʈ�� �����?
    // 1. ���� (ũ��Ƽ��) 
    // 2. Ž�� (���� �� ���� ȸ����)
    // 3. ��� (���ݷ�)
    // 4. �ټ� (ü�� �� ü�� ȸ����)
    // 5. ?? (����)  
    // 6. ??? (���� ����) Ž�� 

}
