using UnityEngine;

public class GameStats : MonoBehaviour
{
    public string Name;
    public int MaxHp;
    public int CurrentHP;
    private bool IsDead;

    virtual public void Start()
    {
        CurrentHP = MaxHp;
        IsDead = false;
    }

    virtual public void Update()
    {
        if (IsDeath())
        {
            Kill();
        }
    }

    virtual public void GetDamage(int Damage)
    {
        CurrentHP -= Damage;
        Debug.Log(Name + " took damage! Current HP: " + CurrentHP);
    }

    virtual public bool IsDeath()
    {
        if (CurrentHP <= 0 && !IsDead)
        {
            IsDead = true;
            return true;
        }
        return false;
    }

    virtual public void Kill()
    {
        Debug.Log(Name + " is being destroyed!");
        Destroy(gameObject);
    }
}