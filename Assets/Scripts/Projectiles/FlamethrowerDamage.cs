using UnityEngine;
using System.Collections.Generic;

public class FlamethrowerDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damagePerTick = 5;
    [Tooltip("How often damage is applied (e.g., 0.2 means 5 times per second).")]
    public float damageInterval = 0.2f;

    [Tooltip("Filter which layers the flamethrower can damage (e.g., Enemy).")]
    public LayerMask damageableLayers;

    // Track targets currently inside the flames and when they can next take damage
    private Dictionary<GameStats, float> targetsInFlame = new Dictionary<GameStats, float>();
    private List<GameStats> keysToRemove = new List<GameStats>();

    void Start()
    {
        // 1. Find the CapsuleCollider on the child object
        CapsuleCollider childCollider = GetComponentInChildren<CapsuleCollider>();

        if (childCollider != null)
        {
            childCollider.isTrigger = true;

            // 2. FORCE a Rigidbody onto this parent object if it doesn't have one.
            // This guarantees Unity forwards trigger messages up the hierarchy!
            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true; // Prevents it from falling or moving physically
                rb.useGravity = false;
            }

            // 3. Just in case Unity still gets confused by the hierarchy, 
            // we inject a helper script directly onto the child collider object.
            GameObject childObj = childCollider.gameObject;
            if (childObj.GetComponent<FlamethrowerTriggerBridge>() == null)
            {
                FlamethrowerTriggerBridge bridge = childObj.AddComponent<FlamethrowerTriggerBridge>();
                bridge.Initialize(this);
            }
        }
        else
        {
            Debug.LogError($"FlamethrowerDamage on {gameObject.name} couldn't find a CapsuleCollider in its children!");
        }
    }

    void Update()
    {
        keysToRemove.Clear();

        // 1. Safely grab the keys out into a separate list first.
        // This isolates the collection so modifying the dictionary won't break the loop.
        List<GameStats> currentTargets = new List<GameStats>(targetsInFlame.Keys);

        // 2. Loop through our snapshot list instead of the live dictionary
        foreach (GameStats target in currentTargets)
        {
            // If the enemy was destroyed or died while in the fire, mark it for cleanup
            if (target == null)
            {
                keysToRemove.Add(target);
                continue;
            }

            // If enough time has passed since their last damage tick
            if (Time.time >= targetsInFlame[target])
            {
                target.GetDamage(damagePerTick);

                // Update the dictionary value safely
                targetsInFlame[target] = Time.time + damageInterval;
            }
        }

        // 3. Clean up any null or dead references safely outside the main loop
        foreach (GameStats key in keysToRemove)
        {
            targetsInFlame.Remove(key);
        }
    }

    // Public methods so our helper bridge script can pass the data here
    public void ProcessTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageableLayers) == 0) return;

        GameStats enemyStats = other.GetComponentInParent<GameStats>();

        if (enemyStats != null && !targetsInFlame.ContainsKey(enemyStats))
        {
            targetsInFlame.Add(enemyStats, Time.time);
        }
    }

    public void ProcessTriggerExit(Collider other)
    {
        GameStats enemyStats = other.GetComponentInParent<GameStats>();

        if (enemyStats != null && targetsInFlame.ContainsKey(enemyStats))
        {
            targetsInFlame.Remove(enemyStats);
        }
    }

    private void OnDisable()
    {
        targetsInFlame.Clear();
    }
}

// --- HELPER BRIDGE SCRIPT ---
// This automatically sits on the child object and redirects hits to the parent script
public class FlamethrowerTriggerBridge : MonoBehaviour
{
    private FlamethrowerDamage mainScript;

    public void Initialize(FlamethrowerDamage parentScript)
    {
        mainScript = parentScript;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mainScript != null) mainScript.ProcessTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (mainScript != null) mainScript.ProcessTriggerExit(other);
    }
}