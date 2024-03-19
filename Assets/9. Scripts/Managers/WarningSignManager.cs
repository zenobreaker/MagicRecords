using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSignInfo
{
    public Vector3 startPos;
 
    public Quaternion quaternion;
    

    public WarningSignInfo(Vector3 startPos, Quaternion quaternion)
    {
        this.startPos = startPos; 
        this.quaternion = quaternion;
    }

  
}

public class WarningSignLineTypeInfo : WarningSignInfo
{
    public float distance;
    public float width;

    public WarningSignLineTypeInfo(Vector3 startPos, Quaternion quaternion) :
        base(startPos, quaternion)
    {

    }

    public float Width{
        set{ this.width = value; }
        get{ return this.width; }
    }
}

public class WaringSingeCircleTypeInfo : WarningSignInfo
{
    public float angle;

    public WaringSingeCircleTypeInfo(Vector3 startPos, Quaternion quaternion) :
        base(startPos, quaternion)
    {
    }

   public float Angle
    {
        set{ this.angle = value; }
        get { return angle; }
    }
}



// �������� ���Ͱ� ���� �߻� ���� �����ϴ� ���� ���� ����� �Ŵ��� 
public class WarningSignManager : MonoBehaviour
{
    public static WarningSignManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public DangerLine dangerLine;


    // ���� ������ ����� �޼ҵ� - ���� ��
    public void CreateWarningSignLines(WarningSignInfo info)
    {
        if (info == null) return;

        var line = Instantiate(dangerLine, info.startPos, info.quaternion);
        if (line == null) return;

        line.CreateGuideSinlgeLine(info);
    }

    public void CreateWarningSignLines(WarningSignInfo[] warningSignInfos)
    {
        foreach(var info in warningSignInfos)
        {
            if (info == null) continue;
            CreateWarningSignLines(info);
        }
    }


    public void CreateWarningSignLines(Vector3 startPos, float distance, Quaternion quaternion)
    {
        if (dangerLine == null) return; 

        WarningSignInfo info = new WarningSignInfo(startPos, quaternion);

        CreateWarningSignLines(info); 
    }

    // ���� ������ ����� �޼ҵ� - ����
    public void CreateWarningSignCircle(WarningSignInfo info)
    {
        if (info == null) return; 

    }
}
