using UnityEngine;

public class Key : Item
{
    [SerializeField] private float useRange = 2.5f;
    [SerializeField] private AudioClip landSound;
    [SerializeField] [Range(0f, 1f)] private float landSoundVolume = 0.8f;

    private bool consumed = false;

    public override void OnInteract()
    {
        if (consumed) return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, useRange))
        {
            Door door = hit.collider.GetComponentInParent<Door>();
            // only act on locked doors — unlocked doors are handled by Door itself
            if (door != null && door.IsLocked && door.TryUnlock(ItemID))
            {
                consumed = true;
                Inventory.Instance.Remove(this);
                DropModel();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.15f);
        Gizmos.DrawSphere(Camera.main.transform.position, useRange);
        Gizmos.color = new Color(1f, 0.85f, 0f, 1f);
        Gizmos.DrawWireSphere(Camera.main.transform.position, useRange);
    }

    void DropModel()
    {
        if (ModelPrefab == null) return;

        Transform cam = Camera.main.transform;
        // spawn in front of and slightly below eye level so player sees it fall
        Vector3 spawnPos = cam.position + cam.forward * 0.6f + Vector3.down * 0.2f;

        // only drop if Floor is below — prevents infinite fall
        int floorMask = 1 << LayerMask.NameToLayer("Floor");
        if (!Physics.Raycast(spawnPos, Vector3.down, 20f, floorMask))
            return;

        GameObject dropped = Instantiate(ModelPrefab, spawnPos, Random.rotation);

        if (dropped.GetComponentInChildren<Collider>() == null)
        {
            SphereCollider sc = dropped.AddComponent<SphereCollider>();
            sc.radius = 0.1f;
        }

        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb == null) rb = dropped.AddComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 2f;

        // inherit player velocity so key doesn't fall behind when moving
        CharacterController cc = Camera.main.transform.root.GetComponent<CharacterController>();
        Vector3 playerVel = cc != null ? cc.velocity : Vector3.zero;

        rb.linearVelocity = playerVel + cam.forward * 0.4f + Vector3.up * 0.2f + cam.right * Random.Range(-0.1f, 0.1f);
        rb.angularVelocity = Random.insideUnitSphere * 3f;

        DroppedItem di = dropped.AddComponent<DroppedItem>();
        di.landSound = landSound;
        di.landVolume = landSoundVolume;
    }
}
