using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserController : MonoBehaviour
{
    [Header("Settings")]
    public float telegraphDuration = 1.5f;
    public float fireDuration = 2.0f;
    [Range(0, 1)] public float telegraphOpacity = 0.05f;
    [Range(0, 1)] public float firingOpacity = 1.0f;

    [Header("Combat")]
    public int laserDamage = 10;
    public float maxDistance = 20f;
    public LayerMask hitLayers;

    [Header("Optional Deathbox")]
    [Tooltip("If assigned, any GameStats inside this box will Die() during the fire duration.")]
    public BoxCollider2D deathZone;

    [Header("Visuals")]
    public float widthMultiplier = 1.0f;
    public Transform laserHolder;
    public LineRenderer beamLine;
    public ParticleSystem endParticles;

    private bool isFiring = false;
    private HashSet<GameStats> hitTargets = new HashSet<GameStats>();

    void Start()
    {
        beamLine.useWorldSpace = true;
        ClearBeam();

        // Ensure the deathZone doesn't accidentally block the laser's own raycast
        if (deathZone != null) deathZone.enabled = false;
    }

    public void FireLaser()
    {
        if (!isFiring) StartCoroutine(LaserRoutine());
    }

    private IEnumerator LaserRoutine()
    {
        isFiring = true;
        hitTargets.Clear();

        float baseWidth = laserHolder.localScale.y * widthMultiplier;

        // --- PHASE 1: TELEGRAPH ---
        float tWidth = baseWidth * 0.2f;
        beamLine.startWidth = tWidth;
        beamLine.endWidth = tWidth;
        SetBeamOpacity(telegraphOpacity);

        float tTimer = 0;
        while (tTimer < telegraphDuration)
        {
            UpdateLaserBeam(false);
            tTimer += Time.deltaTime;
            yield return null;
        }

        // --- PHASE 2: FIRING ---
        beamLine.startWidth = baseWidth;
        beamLine.endWidth = baseWidth;
        SetBeamOpacity(firingOpacity);

        if (endParticles) endParticles.Play();

        float fTimer = 0;
        while (fTimer < fireDuration)
        {
            UpdateLaserBeam(true);
            CheckDeathZone(); // Check the optional square box
            fTimer += Time.deltaTime;
            yield return null;
        }

        ClearBeam();
        isFiring = false;
    }

    void UpdateLaserBeam(bool canDamage)
    {
        Vector3 startPos = laserHolder.position;
        Vector2 direction = -laserHolder.right;

        // Standard Raycast for the visual line
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, hitLayers);
        Vector3 endPoint;

        if (hit.collider != null)
        {
            endPoint = hit.point;
            if (canDamage)
            {
                if (endParticles) endParticles.transform.position = hit.point;

                GameStats stats = hit.collider.GetComponent<GameStats>();
                if (stats != null && !hitTargets.Contains(stats))
                {
                    stats.GetDamage(laserDamage);
                    hitTargets.Add(stats);
                }
            }
        }
        else
        {
            endPoint = startPos + (Vector3)direction * maxDistance;
            if (canDamage && endParticles) endParticles.transform.position = endPoint;
        }

        beamLine.positionCount = 2;
        beamLine.SetPosition(0, startPos);
        beamLine.SetPosition(1, endPoint);
    }

    private void CheckDeathZone()
    {
        if (deathZone == null) return;

        // 1. Setup a filter to only look for your Hit Layers
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(hitLayers);
        filter.useTriggers = true; // Ensures it can find the player even if they use a Trigger collider

        // 2. Prepare a list to hold anything the box touches
        List<Collider2D> results = new List<Collider2D>();

        // 3. Ask the deathZone collider itself what it is touching
        int hitCount = deathZone.Overlap(filter, results);

        // 4. Loop through the hits and apply the kill logic
        for (int i = 0; i < hitCount; i++)
        {
            GameStats stats = results[i].GetComponent<GameStats>();
            if (stats != null)
            {
                stats.Kill();
            }
        }
    }

    private void ClearBeam()
    {
        SetBeamOpacity(0f);
        if (endParticles) endParticles.Stop();
        beamLine.positionCount = 0;
    }

    void SetBeamOpacity(float alpha)
    {
        if (beamLine == null) return;
        Gradient gradient = beamLine.colorGradient;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        for (int i = 0; i < alphaKeys.Length; i++) alphaKeys[i].alpha = alpha;
        gradient.SetKeys(gradient.colorKeys, alphaKeys);
        beamLine.colorGradient = gradient;
    }
}