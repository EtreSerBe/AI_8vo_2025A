using UnityEngine;
using UnityEngine.UI;

public class PickupObject : MonoBehaviour
{
    // Clase base usada para todos los objetos que pueden ser tomados por el jugador.

    [SerializeField]
    protected LayerMask mask_CanBeGrabbedByLayers;

    [SerializeField]
    protected LayerMask mask_InteractWithLayers;

    // La mayoría de los pickup objects desaparecen tras ser tomados, por ejemplo, Rupias,
    // corazones, bombas (del inventario), deku nuts, flechas, etc.
    [SerializeField] 
    protected bool _disappearOnGrab = true;


    // Este método se usará para interacciones de tomar este pickObject. Por ejemplo,
    // otorgar un power-up a quien lo tomó, subir la vida, guardar un objeto en el inventario, etc.
    // En teoría, OnGrab no debe ser overriden por clases hijas, ya que en sí todas deberían tener un 
    // GrabEffect y luego un PostGrab
    protected void OnGrab(GameObject grabber)
    {
        ApplyGrabEffect(grabber);

        PostGrab(grabber);
    }

    // Grab Effect es tal cual, qué pasa a quien tomó este pickupObject. Por ejemplo,
    // le dan un (o unos) item(s) para su inventario? se recupera X vida? X corazones?
    // se abre una puerta? se activa un power-up para quien lo tomó?
    protected virtual void ApplyGrabEffect(GameObject grabber)
    {
        // la función ApplyGrabEffect determina el comportamiento específico de cada clase que herede
        // de PickupObject. Para ello, las clases hijas deben hacer un override a este método.
        Debug.Log($"método ApplyGrabEffect del objeto: {name} chocando contra {grabber.name}");
    }

    // Post grab es "qué le pasa a este pickupObject tras ser tomado?" por ejemplo, 
    // desaparecer, hacer una animación (las rupias se ponen sobre la cabeza de Link un instante),
    // las monedas de mario también. 
    protected virtual void PostGrab(GameObject grabber)
    { 
        
    }

    // Este método se usará para otras interacciones que no sean agarrar el objeto. Por ejemplo,
    // si el pickupObject puede ser destruido al entrar en contacto con X objeto, o al ser atacado, etc.
    protected virtual void OnInteract(GameObject interactor)
    {
        // la función OnTouch determina el comportamiento específico de cada clase que herede de PickupObject.
        // Para ello, las clases hijas deben hacer un override a este método.
        Debug.Log($"método OnInteract del objeto: {name} chocando contra {interactor.name}");
    }



    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered on trigger enter");

        // esto es una sola comprobación para filtrar todas las capas que no nos interesan.
        int otherLayer = 1 << other.gameObject.layer;
        int otherLayerANDmaskValue = (otherLayer & mask_CanBeGrabbedByLayers.value);

        // Checar que sea el player contra quien queremos hacer trigger para que agarre este pickupObject.
        if (otherLayerANDmaskValue != 0)
        {
            // entonces sí es alguien en una capa que puede tomar este pickupObject.
            OnGrab(other.gameObject);
        }
        else // si no se tomó el objeto, entonces checamos si debería interactuar.
        {
            // Filtramos para solo quedarnos con las que nos interesa que este pickup object interactue.
            int otherLayerANDinteractMaskValue = (otherLayer & mask_InteractWithLayers.value);
            if (otherLayerANDinteractMaskValue != 0) // if it is of any layer with which we want to interact.
            {
                // entonces sí es alguien en una capa con la cual queremos que este objeto interactue.
                OnInteract(other.gameObject);
            }
        }
    }
}
