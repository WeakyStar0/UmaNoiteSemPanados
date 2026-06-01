using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private Camera screenCamera;
    [SerializeField] private Transform itemHandPos;
    [SerializeField] private Vector3 inventoryCornerOffset = new Vector3(-0.3f, -0.4f, 0.6f);

    private GameObject heldItemModel;
    private Item currentItem;
    private bool isInHand = false;

    void Start()
    {
        if (screenCamera == null)
            screenCamera = Camera.main;
    }

    void Update()
    {
        SyncWithInventory();

        if (heldItemModel != null && isInHand && itemHandPos != null)
        {
            UpdateHandPosition();
        }
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

        heldItemModel = Instantiate(item.ModelPrefab, itemHandPos);
        DisablePhysics(heldItemModel);

        isInHand = true;
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

        heldItemModel.transform.localPosition = posOffset;
        heldItemModel.transform.localRotation = Quaternion.Euler(rotOffset);
        heldItemModel.transform.localScale = scaleOffset;
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
