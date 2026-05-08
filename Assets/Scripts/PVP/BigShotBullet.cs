using UnityEngine;

public class BigShotBullet : MonoBehaviour
{
    public float maxSize = 2.5f;
    public float maxDamage = 10f;

    public float currentSize = 0.5f;
    public float currentDamage = 1f;

    public float sizeIncreaseSpeed = 0.5f;
    private float damageIncreaseSpeed = 0.5f;

    public float minSizeToShoot = 0.8f;

    public SpriteRenderer sprite;
    private float minSize;
    private float minDamage;

    private Vector2 originalSize;

    public void Start()
    {
        float a = maxSize / sizeIncreaseSpeed;
        damageIncreaseSpeed = maxDamage / a;

        minSize = currentSize;
        minDamage = currentDamage;

        originalSize = transform.localScale;
        Debug.Log(currentSize + " :: " + maxSize);

        
    }

    public void Update()
    {
        if(minSize != currentSize)
        {
            sprite.enabled = true;
        }
        else
        {
            sprite.enabled = false;
        }

        transform.localScale = originalSize * currentSize;
    } 

    public void ChargeBullet()
    {
        if (currentSize  < maxSize) { currentSize += sizeIncreaseSpeed; }
        if (currentDamage < maxDamage) { currentDamage += damageIncreaseSpeed; }
    }

    public bool CanIShoot()
    {
        if(currentSize >= minSizeToShoot)
        {
            return true;
        }
        return false;
    }
    public void ResetBullet()
    {
        currentSize = minSize;
        currentDamage = minDamage;
    }
}
