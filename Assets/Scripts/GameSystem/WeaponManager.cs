using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponType currentWeaponType;

    public WeaponType CurrentWeaponType
    {
        get { return currentWeaponType; }
        set { currentWeaponType = value; }
    }
}
