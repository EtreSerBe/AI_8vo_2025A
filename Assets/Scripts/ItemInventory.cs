using System.Collections.Generic;
using UnityEngine;

// Script usado para tener una colección de objetos según un ID.
public class ItemInventory : MonoBehaviour
{

    // Cada Key-Value-Pair representa la cantidad (Value) de objetos del tipo (Key).
    // Por ejemplo, items["bomb"] = 5 significa que tienes 5 bombas en el inventario.
    [SerializeField]
    protected Dictionary<string, int> items = new Dictionary<string, int>();

    // Añade 1 item por defecto, para que sea posible llamarla como: AddItemAmount("bomb");
    // y así añada 1 bomba sin tener que poner el 1 explícitamente.
    public void AddItemAmount(string itemName, int amount = 1)
    {
        if(items.ContainsKey(itemName))
        {
            // TO-DO: Hacer que esté limitado por el tipo de objeto que se está añadiendo,
            // por ejemplo, si tomas 5 bombas, pero solo puedes cargar 3, que se quede en 3.
            // lo ideal para esto sería tener un scriptable Object que nos diga el límite de cada item type.
            items[itemName] += amount; // si ya tenía items de este tipo, se le suman.
        }
        else
            items[itemName] = amount; // si aún no tenía items de este tipo, se pone como su valor inicial.
    }

    public bool RemoveItemAmount(string itemName, int amount = 1)
    {
        if (items.ContainsKey(itemName))
        {
            // Si no tiene suficientes items de este tipo, imprimir un error y salir de la función.
            if (items[itemName] < amount) 
            {
                Debug.LogError($"Error, no se pueden quitar: {amount} items del tipo " +
                    $"{itemName} en este item Inventory. Solo tiene {items[itemName]}, favor de verificar.");
                return false;
            }

            items[itemName] -= amount; // si ya tenía items de este tipo, se le restan.
            return true;
        }
        else
            Debug.LogError($"Error, no hay un item de nombre {itemName} en este item Inventory.");
        return false;
    }

    public void EmptyItemType(string itemName) 
    {
        if (items.ContainsKey(itemName))
            items[itemName] = 0;
        else
            Debug.LogWarning($"Se trató de vaciar el tipo de item {itemName} de este inventario, " +
                $"pero no se tiene registro de dicho item.");
    }

}
