using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // TO-DO: Sacar todas las variables que ser�n compartidas por cada
    // tipo de bala y ponerlas en un ScriptableObject, para no duplicar
    // dichas variables por cada bala que sea spawneada.

    [SerializeField]
    protected LayerMask mask;

    // qu� tanto da�o va a hacer esta bala al colisionar contra algo.
    [SerializeField]
    protected float damage;

    // Tipo(s) de da�o que causar� este ataque. Por defecto debe tener uno
    [SerializeField]
    protected List<DamageType> damageTypes = new List<DamageType>() 
    { DamageType.Generic };

    public float GetDamage() { return damage; }

    // lo �nico que necesita saber una bala es saber cu�ndo choca.
    private void OnTriggerEnter(Collider other)
    {
        // queremos que s� choque contra Enemigos (Enemy), Paredes (Wall), Obst�culos (Obstacle)
        if (IsHitting(other.gameObject))
        {
            // Debug.Log("Choque con algo en la capa" + LayerMask.LayerToName(other.gameObject.layer) );
            OnHit(other.gameObject);

            // Vamos a destruir nuestra bala, porque la mayor�a de las balas se destruyen al tocar algo.
            // Si necesit�ramos una bala que se comporte distinto, le podemos hacer override a OnTriggerEnter
            // en la clase espec�fica de esa bala.
            PostHit(other.gameObject);
        }
    }

    // Funci�n sobreescribible que determina c�mo se comporta este tipo de bala al chocar.
    // aqu� se puede modificar qu� efectos aplica al chocar ciertos tipos de layers, 
    // hacer da�o, aplicar efectos de estado, rebotar contra paredes, etc.
    protected virtual void OnHit(GameObject objectHit)
    {
        Debug.Log($"la bala {name} Colision� con: {objectHit.name}");

        // Lo m�s com�n ser�a checar si colisionamos contra un DamageableEntity
        // y si s� lo hicimos, entonces le bajamos HP.
        // Idealmente, aqu� usar�amos una Interface, pero eso ser�a sobre-complicarlos.
        // As� que lo que har� ser� obtener el tipo de script espec�fico seg�n la layer en que est�n.
        if(Constants.PlayerLayer == objectHit.layer)
        {
            if(objectHit.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
            {
                // Si s� tiene ese componente, entonces le podremos hacer da�o seg�n lo que PlayerStats nos diga.
                // La bala �nicamente le dir� el da�o "base", su tipo, etc.
            }
            // Si no ten�a ese script, muy probablemente es un error, as�
            // que imprimimos un debug de warning.
            Debug.LogWarning($"El player {objectHit.name} deber�a tener un componente PlayerStats pero no lo ten�a");
        }
        else if (Constants.WallLayer == objectHit.layer)
        {

        }
        else if (Constants.EnemyLayer == objectHit.layer)
        {

        }
        else if (Constants.ObstacleLayer == objectHit.layer)
        {

        }


    }

    // protected bool CollidingWithPlayer()


    // Funci�n sobreescribible que determina c�mo se comporta este tipo de bala
    // despu�s de chocar. Generalmente se usa para destruir las balas, spaw
    // Muchas balas simplemente se destruir�n, pero otras no.
    // B�sicamente es para evitar tener que llamar destroy en todas las dem�s clases hijas.
    protected virtual void PostHit(GameObject objectHit)
    {
        Destroy(gameObject);
    }

    // Checa si el objectToCollide pertenece a una layer que nos interesa.
    protected bool IsHitting(GameObject objectToCollide)
    {
        // Obtenemos la layer a la que pertenece objectToCollide.
        var maskValue = 1 << objectToCollide.layer;
        // sacamos el valor de pasar esa layer por la m�scara de las layers que s� nos interesan.
        var maskANDmaskValue = (maskValue & mask.value);

        // esto es una sola comprobaci�n para filtrar todas las capas que no nos interesan.
        return maskANDmaskValue > 0;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("colisi�n con algo en la capa" + LayerMask.LayerToName(collision.gameObject.layer));

    //    // queremos que s� choque contra Enemigos (Enemy), Paredes (Wall), Obst�culos (Obstacle)
    //    var maskValue = 1 << collision.gameObject.layer;
    //    if (~(maskValue & mask.value) == 1)
    //    {
    //        Debug.Log("Choque con algo en la capa" + LayerMask.LayerToName(collision.gameObject.layer));
    //    }
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
