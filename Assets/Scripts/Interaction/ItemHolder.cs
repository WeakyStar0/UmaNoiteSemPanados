using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private Camera screenCamera;
    [SerializeField] private Transform itemHandPos;
    [SerializeField] private Vector3 inventoryCornerOffset = new Vector3(-0.3f, -0.4f, 0.6f);

    [Header("Draw Animation")]
    [SerializeField] private Vector3 drawInOffset = new Vector3(0.25f, -0.35f, 0f);
    [SerializeField] private float drawInDuration = 0.35f;
    [SerializeField] private float drawInOvershoot = 1.8f;

    private GameObject heldItemModel;
    private Item currentItem;
    private bool isInHand = false;
    private bool isDrawing = false;
    private float drawStartTime;

    void Start()
    {
        if (screenCamera == null)
            screenCamera = Camera.main;
    }

    void Update()
    {
        SyncWithInventory();

        bool blocked = PauseManager.Instance != null && (PauseManager.Instance.IsPaused || PauseManager.Instance.IsJustResumed);
        if (!blocked)
        {
            if (currentItem != null && Mouse.current.leftButton.wasPressedThisFrame)
                currentItem.OnUse();

            if (currentItem != null && Keyboard.current.eKey.wasPressedThisFrame)
                currentItem.OnInteract();

            if (currentItem != null && currentItem.Droppable && Keyboard.current.qKey.wasPressedThisFrame)
                DropCurrentItem();
        }

        if (heldItemModel != null && isInHand && itemHandPos != null)
            UpdateHandPosition();
    }

    void SyncWithInventory()
    {
        Item selectedItem = Inventory.Instance.GetSelectedItem();
        if (selectedItem != currentItem)
        {
            DisplayItem(selectedItem);
        }
    }

    public void DisplayItem(Item item)
    {
        if (item == null)
        {
            HideItem();
            return;
        }

        HideItem();
        currentItem = item;

        if (item.ModelPrefab == null) return;

        GameObject pivot = new GameObject("ItemPivot");
        pivot.transform.SetParent(itemHandPos);

        GameObject modelInstance = Instantiate(item.ModelPrefab, pivot.transform);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.identity;
        modelInstance.transform.localScale = Vector3.one;

        heldItemModel = pivot;
        DisablePhysics(modelInstance);
        item.OnModelSpawned(modelInstance);

        foreach (Renderer r in modelInstance.GetComponentsInChildren<Renderer>())
            r.shadowCastingMode = ShadowCastingMode.Off;

        isInHand = true;
        isDrawing = true;
        drawStartTime = Time.time;
        UpdateHandPosition();
    }

    public void MoveToHand()
    {
        if (heldItemModel == null || itemHandPos == null) return;

        isInHand = true;
        heldItemModel.transform.SetParent(itemHandPos);
        UpdateHandPosition();
    }

    public void MoveToInventory()
    {
        if (heldItemModel == null) return;

        isInHand = false;
        heldItemModel.transform.SetParent(screenCamera.transform);
        SetInventoryPosition();
    }

    void SetInventoryPosition()
    {
        heldItemModel.transform.localPosition = inventoryCornerOffset;
        heldItemModel.transform.localRotation = Quaternion.identity;
        heldItemModel.transform.localScale = Vector3.one * 0.15f;
    }

    void UpdateHandPosition()
    {
        if (heldItemModel == null || currentItem == null) return;

        Vector3 posOffset = (currentItem.DebugPositionOffset != Vector3.zero) ? currentItem.DebugPositionOffset : currentItem.HandPositionOffset;
        Vector3 rotOffset = (currentItem.DebugRotationOffset != Vector3.zero) ? currentItem.DebugRotationOffset : currentItem.HandRotationOffset;
        Vector3 scaleOffset = (currentItem.DebugScaleOffset != Vector3.one) ? currentItem.DebugScaleOffset : currentItem.HandScaleOffset;

        if (isDrawing)
        {
            float t = Mathf.Clamp01((Time.time - drawStartTime) / drawInDuration);
            if (t >= 1f) isDrawing = false;
            float ease = EaseOutBack(t, drawInOvershoot);
            heldItemModel.transform.localPosition = Vector3.LerpUnclamped(posOffset + drawInOffset, posOffset, ease);
        }
        else
        {
            heldItemModel.transform.localPosition = posOffset;
        }

        heldItemModel.transform.localRotation = Quaternion.Euler(rotOffset);
        heldItemModel.transform.localScale = scaleOffset;
    }

    float EaseOutBack(float t, float overshoot)
    {
        float c3 = overshoot + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + overshoot * Mathf.Pow(t - 1f, 2f);
    }

    void DropCurrentItem()
    {
        Item item = currentItem;
        HideItem();
        Inventory.Instance.Remove(item);

        Transform cam = Camera.main.transform;
        CharacterController cc = cam.root.GetComponent<CharacterController>();
        Vector3 playerVel = cc != null ? cc.velocity : Vector3.zero;
        Vector3 spawnPos = cam.position + cam.forward * 0.6f + Vector3.down * 0.2f;
        Vector3 velocity = playerVel + cam.forward * 2f + Vector3.up * 1f;

        item.Drop(spawnPos, velocity);
    }

    void HideItem()
    {
        if (heldItemModel != null)
            Destroy(heldItemModel);

        heldItemModel = null;
        currentItem = null;
        isInHand = false;
    }

    void DisablePhysics(GameObject item)
    {
        Collider col = item.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

public Item GetCurrentItem() => currentItem;
    public bool IsInHand => isInHand;
}
