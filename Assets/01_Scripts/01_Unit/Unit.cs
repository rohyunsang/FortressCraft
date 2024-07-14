using UnityEngine;

public class Unit : MonoBehaviour, IUnitStats
{
    // Properties based on the fields from UnitData
    // under data will change Networked :: Fix 
    // under code will fix protected  :: Fix
    #region Stats
    public int Index { get; set; }
    public string Name { get; set; }
    public string KRName { get; set; }
    public int Grade { get; set; }
    public int Class { get; set; }
    public int Cost { get; set; }
    public float HP { get; set; }
    public float MP { get; set; }
    public float AttackPower { get; set; }
    public float Defense { get; set; }
    public float AttackSpeed { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackRange { get; set; }
    public string DesText { get; set; }

    public Transform target { get; set; }

    
    #endregion
    /*
    protected void Initialize(UnitData data)
    {
        Index = data.Index;
        Name = data.Name;
        Grade = data.Grade;
        Class = data.Class;
        Cost = data.Cost;
        KRName = data.KRName;
        HP = data.HP;
        MP = data.MP;
        AttackPower = data.AttackPower;
        Defense = data.Defense;
        AttackSpeed = data.AttackSpeed;
        MoveSpeed = data.MoveSpeed;
        AttackRange = data.AttackRange;
        DesText = data.DesText;
    }
    */
    protected virtual void MoveToTarget()
    {

    }

    protected virtual bool Attack()
    {
        return false;
    }

    protected virtual void Die()
    {
    }

    protected void UseSkill()
    {
    }

    protected void SetStat(string statName, float value)
    {
        switch (statName)
        {
            case "HP":
                HP = value;
                break;
            case "MP":
                MP = value;
                break;
            case "AttackPower":
                AttackPower = value;
                break;
            case "Defense":
                Defense = value;
                break;
            case "AttackSpeed":
                AttackSpeed = value;
                break;
            case "MoveSpeed":
                MoveSpeed = value;
                break;
            default:
                Debug.LogError("Invalid stat name: " + statName);
                break;
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}