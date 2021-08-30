using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> keySlots;
    [SerializeField] List<ItemSlot> bookSlots;

    List<List<ItemSlot>> allSlots;

    public event Action onUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, keySlots, bookSlots };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "KEY ITEMS", "BOOKS"
    };

    public List<ItemSlot> GetSlotByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int itemIndex, Piece selectedPiece)
    {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPiece);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
            slots.Remove(itemSlot);

        onUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
       return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count
    {
        get => count;
        set => count = value;
    }
}