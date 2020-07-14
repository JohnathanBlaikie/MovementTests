using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalScript : MonoBehaviour
{
    public GameObject[] Weapon;
    public enum WEAPONTYPE { Melee, Hitscan, Projectile}
    public WEAPONTYPE weaponType;
    public float damage, headshotMultiplier;
    public float maxAmmo, fireRate, reloadSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
