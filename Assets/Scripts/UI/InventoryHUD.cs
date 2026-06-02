using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryHUD : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public RawImage displayImage;
        public Image selectionBorder;
        public Transform modelSpawnPoint;
        public Camera previewCamera;
    }

    [SerializeField] private InventorySlot[] slots = new InventorySlot[4];
    [SerializeField] private string previewLayerName = "ItemPreview";

    private int previewLayer;
    private GameObject[] spawnedModels = new GameObject[4];

    void Awake()
    {
        previewLayer = LayerMask.NameToLayer(previewLayerName);
    }

    void Start()
    {
        Inventory.Instance.OnChanged += RefreshHUD;
        RefreshHUD();
    }

    void OnDestroy()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnChanged -= RefreshHUD;
    }

    void Update()
    {
        List<Item> items = Inventory.Instance.GetAll();
        for (int i = 0; i < slots.Length; i++)
        {
            if (spawnedModels[i] == null) continue;
            Item item = i < items.Count ? items[i] : null;
            if (item == null) continue;
            spawnedModels[i].transform.localPosition = item.HotbarPositionOffset;
            spawnedModels[i].transform.localRotation = Quaternion.Euler(item.HotbarRotationOffset);
            spawnedModels[i].transform.localScale = item.HotbarScaleOffset;
        }
    }

    void RefreshHUD()
    {
        List<Item> items = Inventory.Instance.GetAll();
        int selected = Inventory.Instance.SelectedIndex;

        for (int i = 0; i < slots.Length; i++)
        {
            Item item = i < items.Count ? items[i] : null;
            UpdateSlot(i, item);
            if (slots[i].selectionBorder != null)
                slots[i].selectionBorder.enabled = (i == selected && item != null);
        }
    }

    void UpdateSlot(int index, Item item)
    {
        if (spawnedModels[index] != null)
        {
            Destroy(spawnedModels[index]);
            spawnedModels[index] = null;
        }

        if (item == null || item.ModelPrefab == null)
        {
            if (slots[index].displayImage != null)
                slots[index].displayImage.color = new Color(1, 1, 1, 0);
            return;
        }

        if (slots[index].displayImage != null)
            slots[index].displayImage.color = Color.white;

        GameObject pivot = new GameObject("HotbarPivot");
        pivot.transform.SetParent(slots[index].modelSpawnPoint);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = Quaternion.identity;
        pivot.transform.localScale = Vector3.one;

        GameObject model = Instantiate(item.ModelPrefab, pivot.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        SetLayerRecursive(model, previewLayer);
        DisablePhysics(model);

        spawnedModels[index] = pivot;
    }

    void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    void DisablePhysics(GameObject go)
    {
        foreach (var col in go.GetComponentsInChildren<Collider>())
            col.enabled = false;
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
    }
}
