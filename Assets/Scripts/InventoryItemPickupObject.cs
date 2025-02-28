using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemInventoryConstants;

// clase para todos los objetos que se añadan al inventario al tomarse. Por ejemplo,
// en Zelda: Bombas, Deku nuts, flechas, llaves, etc.
public class InventoryItemPickupObject : PickupObject
{
    
    [SerializeField]
    protected InventoryItemsEnum _inventoryItemType = InventoryItemsEnum.None;

    // Cantidad de este item que añadiremos al inventario al ser tomados.
    [SerializeField]
    protected int _amountToGrant = 1;

    [SerializeField]
    protected float _displayOverHeadTime = 1.0f;

    [SerializeField]
    protected float _displayOverHeadMovementRange = 1.0f;

    private Coroutine _displayItemOverCharacterHeadCoroutine;

    // Hacemos algunas corrutinas útiles para objetos agarrables. Por ejemplo,
    // aparecer el item sobre la cabeza del player, o hacer una animación como al tomar una pieza de corazón
    // o item de un cofre en zelda.
    // Parámetros: shownTime->cuánto tiempo mostrarlo;
    // movementRange-> movimiento máximo que se desplaza hacia arriba conforme el tiempo avanza.
    protected virtual IEnumerator DisplayItemOverCharacterHead(float shownTime, float movementRange, GameObject grabber)
    {
        float transcurredTime = 0.0f;
        // Movemos el gameobject de este item a la posición "PositionOverHead" del personaje.
        Transform positionOverHeadTransform = grabber.transform.Find("PositionOverHead");
        if (positionOverHeadTransform == null)
        {
            Debug.LogError($"Gameobject {grabber.name} does not have a PositionOverHead child gameObject.");
            yield return null;
        }

        // TO-DO: podemos reproducir un efecto de sonido aquí.
        
        // ahora sí, movemos nuestro item al positionOverHead
        transform.position = positionOverHeadTransform.position;
        Vector3 verticalDisplacement = Vector3.zero;

        // Mientras no haya concluido el tiempo que se mostrará el item
        while(transcurredTime < shownTime)
        {
            transcurredTime += Time.deltaTime;
            // cada frame lo reubicamos sobre el player, para que siga su movimiento.
            verticalDisplacement = transform.up * movementRange * transcurredTime;
            transform.position = positionOverHeadTransform.position + verticalDisplacement;
            yield return null;
        }

        Debug.Log("destruyendo este item");
        // una vez acabe la corrutina, destruimos al game object.
        Destroy(gameObject);
    }


    protected override void ApplyGrabEffect(GameObject grabber)
    {
        base.ApplyGrabEffect(grabber); // por ahora solo imprime que sí se entró a este método.

        // Añadimos una llave al inventario del jugador y desaparecemos este gameobject.
        // primero, corroboramos que el jugador tenga un script de inventario
        //try get component grabber.GetComponent
        if (grabber.TryGetComponent<ItemInventory>(out var itemInventory))
        {
            // si sí tiene el componente, ya está guardado en la variable itemInventory y lo podemos usar
            // como lo consideremos necesario.
            Debug.Log($"añadiendo un objeto de tipo: {EnumToString[_inventoryItemType]} al inventario");
            // Así que le añadimos los _amountToGrant elementos del tipo _inventoryItemType a este inventario.
            itemInventory.AddItemAmount(EnumToString[_inventoryItemType], _amountToGrant);
        }
        else
            Debug.LogError($"the grabber {grabber.name} does not have an ItemInventory script attached");

        // Recordatorio:
        // al terminar de ejecutarse esta función de ApplyGrabEffect, se ejecutará el PostGrab de esta clase.
    }

    protected override void PostGrab(GameObject grabber)
    {
        base.PostGrab(grabber);

        // En este caso, los InventoryItems desaparecen después de tomarlos.
        // Específicamente para mi implementación, me gustaría hacer que el item aparezca por 
        // un corto tiempo encima del personaje del jugador, como en Zelda Ocarina of Time.
        // TO-DO: Hacer que aparezcan encima de la cabeza del personaje.

        // Guardamos referencia a la corrutina, en caso de que la queramos detener por alguna razón.
        _displayItemOverCharacterHeadCoroutine =
            StartCoroutine(DisplayItemOverCharacterHead(_displayOverHeadTime, _displayOverHeadMovementRange, grabber));

        // Finalmente, destruir este objeto para que ya no aparezca en la escena.
        // Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
