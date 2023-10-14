using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    protected UnitData data;
    protected Transform transform;
    protected int currentHealth;

    protected string uid;
    protected int level;

    public Unit(UnitData _data)
    {
        data = _data;
        currentHealth = _data.healthpoints;

        // Instantiate a GameObject based on the unit's code from a prefab
        GameObject g = GameObject.Instantiate(data.prefab) as GameObject;
        transform = g.transform;
    }

    public void SetPosition(Vector3 _position)
    {
        transform.position = _position;
    }

    public virtual void Place()
    {
        transform.GetComponent<BoxCollider>().isTrigger = false;
    }

    public void LevelUp()
    {
        level += 1;
    }



    public UnitData Data { get => data; }
    public string Code { get => data.code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => data.healthpoints; }

    public string Uid { get => uid; }
    public int Level { get => level; }
}
