using System.Collections.Generic;
using UnityEngine;

public static class ItemInventoryConstants
{
    public enum InventoryItemsEnum
    {
        None = 0,
        RegularKey,
        Bomb,
        Potion,
        // Money
    }

    public static string Bomb = nameof(Bomb);
    public static string RegularKey = nameof(RegularKey);
    public static string Potion = nameof(Potion);

    public static readonly Dictionary<InventoryItemsEnum, string> EnumToString = new Dictionary<InventoryItemsEnum, string>()
    {
        { InventoryItemsEnum.None, "NONE" },
        { InventoryItemsEnum.Bomb, Bomb },
        { InventoryItemsEnum.RegularKey, RegularKey },
        { InventoryItemsEnum.Potion, Potion }
    };

    public static readonly Dictionary<string, InventoryItemsEnum> StringToEnum = new Dictionary<string, InventoryItemsEnum>()
    {
        { "NONE", InventoryItemsEnum.None },
        { Bomb, InventoryItemsEnum.Bomb },
        { RegularKey, InventoryItemsEnum.RegularKey },
        { Potion, InventoryItemsEnum.Potion }
    };

}
