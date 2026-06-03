using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public event System.Action OnChanged;
    public int SelectedIndex => selectedIndex;
    public bool IsFull => items.Count >= maxSlots;

    public void SetMaxSlots(int count) { maxSlots = count; }

    private int maxSlots = 4;

    private List<Item> items = new List<Item>();
    private int selectedIndex = -1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(Item item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.LogWarning("Inventory full!");
            return;
        }
        items.Add(item);
        Debug.Log($"Added {item.DisplayName} to inventory. Total: {items.Count}");

        if (selectedIndex == -1)
            SelectItem(0);
        else
            OnChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (items.Count == 0)
            selectedIndex = -1;
        else if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;
        OnChanged?.Invoke();
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
        {
            selectedIndex = index;
            OnChanged?.Invoke();
        }
    }

    public void SelectNext()
    {
        if (items.Count == 0) return;
        selectedIndex = selectedIndex >= items.Count - 1 ? -1 : selectedIndex + 1;
        OnChanged?.Invoke();
    }

    public void SelectPrev()
    {
        if (items.Count == 0) return;
        selectedIndex = selectedIndex <= -1 ? items.Count - 1 : selectedIndex - 1;
        OnChanged?.Invoke();
    }
}

