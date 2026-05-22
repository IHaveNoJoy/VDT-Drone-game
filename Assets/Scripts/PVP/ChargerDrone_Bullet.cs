using UnityEngine;

public class ChargerDrone_Bullet : MonoBehaviour
{
    public float totalChargeTime = 100f;
    public float currentCharge = 0f;
    public float chargeSpeed = 1f;
    public float angle = 0f;

    public Color colorWhenReady;
    public Color normalColor;
    public SpriteRenderer sprite;

    private void Start()
    {
        if (!GetComponentInParent<ChargerDrone>().isPlayer1)
        {
            angle -= 180f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        ChangeOpacity();
    }

    public void Charge()
    {
        if (CanIBeShot() == false)
        {
            currentCharge += chargeSpeed;
        }
    }

    public void ResetCharge()
    {
        currentCharge = 0f;
    }

    public bool CanIBeShot()
    {
        if(currentCharge < totalChargeTime)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ChangeOpacity()
    {
        if (CanIBeShot())
        {
            sprite.color = colorWhenReady;
        }
        else
        {
            sprite.color = normalColor;
        }

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentCharge/100);
    }
}
