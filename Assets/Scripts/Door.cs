using UnityEngine;

public class Door : MonoBehaviour
{
    // Nos lleva de un cuarto a otro, 

    // Tenemos que saber cu�les son los dos cuartos que est� conectando.
    Room sideA;
    Room sideB;

    // Necesitamos colliders para detectar la colisi�n para "usar la puerta" y desde qu� direcci�n.
    Collider sideACollider;
    Collider sideBCollider;

    public Door(Room in_sideA, Room in_sideB)
    {
        sideA = in_sideA;
        sideB = in_sideB;
    }

    // puede estar cerrada o abierta

    // puede requerir una llave o no

    // puede estar cerrada de un lado pero de otro no.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
