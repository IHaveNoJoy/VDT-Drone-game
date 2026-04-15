using UnityEngine;

public class GameStats : MonoBehaviour
{
    [Header("Data")]
    public EnemyData data;

    public int CurrentHP;
    public bool IsDead;

    virtual public void Start()
    {
        if (data != null)
        {
            CurrentHP = data.maxHp;
        }

        IsDead = false;
    }

    virtual public void Update()
    {
        if (IsDead)
        {
            Kill();
        }
    }

    virtual public void GetDamage(int Damage)
    {
        if (IsDead) return;

        CurrentHP -= Damage;

        if (CurrentHP <= 0)
        {
            IsDead = true;
        }
    }

    virtual public void Kill()
    {
        Destroy(gameObject);
    }
}