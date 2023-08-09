using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage 
{
    public void Damage(int _damage, bool isCrit = false);

    public void Damage(int _damage, Vector3 _targetPos, bool isCrit = false);
}
