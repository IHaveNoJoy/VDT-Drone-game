using UnityEngine;

public class GameStats : MonoBehaviour
{
    public string Name;
    public float MaxHp;
    public float CurrentHP;
    private bool IsDead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    {
        CurrentHP = MaxHp;
        IsDead = false;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (IsDead)
        {
            Kill();
        }
    }

    virtual public void GetDamage(float Damage)
    {
        CurrentHP -= Damage;
    }

    virtual public bool IsDeath()
    {
        if (CurrentHP <= 0 && !IsDead) { IsDead = true; return true; }
        return false;
    }

    virtual public void Kill()
    {
        Destroy(this);
    }
}
