using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public LayerMask layers;
    public GameObject Crosshair;
    public LineRenderer line;
    private Vector3 hitPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (CastRay())
        {
            SetLine(transform.position, hitPoint);
            SetCrosshairPosition();
        }
        else
        {
            SetLine(transform.position, transform.position);
            Crosshair.SetActive(false);
        }
    }

    public bool CastRay()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, 500, layers))
        {
            hitPoint = hit.point;
            return true;
        }
        return false;
    }

    public void SetLine(Vector3 pointA, Vector3 pointB)
    {
        line.SetPosition(0, pointA);
        line.SetPosition(1, pointB);
    }
    public void SetCrosshairPosition()
    {
        Crosshair.transform.position = hitPoint;
        Crosshair.SetActive(true);
    }
}
