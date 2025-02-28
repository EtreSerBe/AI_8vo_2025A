using UnityEngine;

public class Door : MonoBehaviour
{
    // Nos lleva de un cuarto a otro, 

    // Tenemos que saber cuáles son los dos cuartos que está conectando.
    Room sideA;
    Room sideB;

    // Necesitamos colliders para detectar la colisión para "usar la puerta" y desde qué dirección.
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

    private void OnTriggerEnter(Collider other)
    {
        //LayerMask pickupLayerMask = LayerMask.GetMask("Player", "Bullet");

        //if (other.gameObject.layer == LayerMask.NameToLayer("Player")
        //    || other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        //{ 
        //    // entonces chocamos con el player.

        //}
    }
}
