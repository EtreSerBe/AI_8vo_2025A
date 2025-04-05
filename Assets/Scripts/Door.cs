using UnityEngine;

public class Door 
{
    // Nos lleva de un cuarto a otro, 

    // Tenemos que saber cuáles son los dos cuartos que está conectando.
    public Room sideA;
    public Room sideB;

    // Necesitamos colliders para detectar la colisión para "usar la puerta" y desde qué dirección.
    // Collider sideACollider;
    // Collider sideBCollider;

    public bool ConnectsToRoom(Room room)
    {
        return (sideA == room || sideB == room);
    }

    public void LockDoor(byte keyId)
    {
        _keyId = keyId;
    }

    public byte _keyId;
    public Door(Room in_sideA, Room in_sideB)
    {
        sideA = in_sideA;
        sideB = in_sideB;
    }

    // puede estar cerrada o abierta

    // puede requerir una llave o no

    // puede estar cerrada de un lado pero de otro no.
}
