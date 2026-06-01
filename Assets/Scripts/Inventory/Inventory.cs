using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private List<Item> items = new List<Item>();
    private int selectedIndex = -1;
    private const int MaxSlots = 4;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(Item item)
    {
        if (items.Count >= MaxSlots)
        {
            Debug.LogWarning("Inventory full!");
            return;
        }
        items.Add(item);
        Debug.Log($"Added {item.DisplayName} to inventory. Total: {items.Count}");

        if (selectedIndex == -1)
            SelectItem(0);
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (items.Count == 0)
            selectedIndex = -1;
        else if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;
    }

    public List<Item> GetAll()
    {
        return new List<Item>(items);
    }

    public int GetCount()
    {
        return items.Count;
    }

    public Item GetSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < items.Count)
            return items[selectedIndex];
        return null;
    }

    public void SelectItem(int index)
    {
        if (index >= 0 && index < items.Count)
            selectedIndex = index;
    }

    public void SelectNext()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % items.Count;
        Debug.Log($"Selected item {selectedIndex}: {items[selectedIndex].DisplayName}");
    }

    public void SelectPrev()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        Debug.Log($"Selected item {selectedIndex}: {items[selectedIndex].DisplayName}");
    }
}

