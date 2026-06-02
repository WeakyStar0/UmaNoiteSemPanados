using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private float detectionRadius = 0.1f;
    [SerializeField] private ReticleUI reticleUI;
    [SerializeField] private ItemHolder itemHolder;

    private Item hoveredItem;

    void Update()
    {
        RaycastForItems();
    }

    void RaycastForItems()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        bool hitItem = Physics.SphereCast(ray, detectionRadius, out RaycastHit hit, pickupRange);

        if (hitItem && hit.collider.CompareTag("Pickable"))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null && item != hoveredItem)
            {
                hoveredItem = item;
                reticleUI.ShowPickup(item.DisplayName);

                Outline outline = item.GetComponent<Outline>();
                if (outline != null) outline.enabled = true;
            }
        }
        else
        {
            if (hoveredItem != null)
            {
                Outline outline = hoveredItem.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;

                reticleUI.HidePickup();
                hoveredItem = null;
            }
        }
    }

    public void TryPickup()
    {
        if (hoveredItem == null) return;

        Item itemToPickup = hoveredItem;
        reticleUI.HidePickup();
        hoveredItem = null;

        Collider col = itemToPickup.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer rend = itemToPickup.GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        itemToPickup.gameObject.SetActive(false);

        Inventory.Instance.Add(itemToPickup);
        itemToPickup.OnPickup();
    }

    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        Vector3 origin = mainCamera.transform.position;
        Vector3 end = origin + mainCamera.transform.forward * pickupRange;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, detectionRadius);
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawWireSphere(end, detectionRadius);
    }
}
