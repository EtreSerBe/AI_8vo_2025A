using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // TO-DO: Sacar todas las variables que serán compartidas por cada
    // tipo de bala y ponerlas en un ScriptableObject, para no duplicar
    // dichas variables por cada bala que sea spawneada.

    [SerializeField]
    protected LayerMask mask;

    // qué tanto daño va a hacer esta bala al colisionar contra algo.
    [SerializeField]
    protected float damage;

    // Tipo(s) de daño que causará este ataque. Por defecto debe tener uno
    [SerializeField]
    protected List<DamageType> damageTypes = new List<DamageType>() 
    { DamageType.Generic };

    public float GetDamage() { return damage; }

    // lo único que necesita saber una bala es saber cuándo choca.
    private void OnTriggerEnter(Collider other)
    {
        // queremos que sí choque contra Enemigos (Enemy), Paredes (Wall), Obstáculos (Obstacle)
        if (IsHitting(other.gameObject))
        {
            // Debug.Log("Choque con algo en la capa" + LayerMask.LayerToName(other.gameObject.layer) );
            OnHit(other.gameObject);

            // Vamos a destruir nuestra bala, porque la mayoría de las balas se destruyen al tocar algo.
            // Si necesitáramos una bala que se comporte distinto, le podemos hacer override a OnTriggerEnter
            // en la clase específica de esa bala.
            PostHit(other.gameObject);
        }
    }

    // Función sobreescribible que determina cómo se comporta este tipo de bala al chocar.
    // aquí se puede modificar qué efectos aplica al chocar ciertos tipos de layers, 
    // hacer daño, aplicar efectos de estado, rebotar contra paredes, etc.
    protected virtual void OnHit(GameObject objectHit)
    {
        Debug.Log($"la bala {name} Colisionó con: {objectHit.name}");

        // Lo más común sería checar si colisionamos contra un DamageableEntity
        // y si sí lo hicimos, entonces le bajamos HP.
        // Idealmente, aquí usaríamos una Interface, pero eso sería sobre-complicarlos.
        // Así que lo que haré será obtener el tipo de script específico según la layer en que estén.
        if(Constants.PlayerLayer == objectHit.layer)
        {
            if(objectHit.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
            {
                // Si sí tiene ese componente, entonces le podremos hacer daño según lo que PlayerStats nos diga.
                // La bala únicamente le dirá el daño "base", su tipo, etc.
            }
            // Si no tenía ese script, muy probablemente es un error, así
            // que imprimimos un debug de warning.
            Debug.LogWarning($"El player {objectHit.name} debería tener un componente PlayerStats pero no lo tenía");
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


    // Función sobreescribible que determina cómo se comporta este tipo de bala
    // después de chocar. Generalmente se usa para destruir las balas, spaw
    // Muchas balas simplemente se destruirán, pero otras no.
    // Básicamente es para evitar tener que llamar destroy en todas las demás clases hijas.
    protected virtual void PostHit(GameObject objectHit)
    {
        Destroy(gameObject);
    }

    // Checa si el objectToCollide pertenece a una layer que nos interesa.
    protected bool IsHitting(GameObject objectToCollide)
    {
        // Obtenemos la layer a la que pertenece objectToCollide.
        var maskValue = 1 << objectToCollide.layer;
        // sacamos el valor de pasar esa layer por la máscara de las layers que sí nos interesan.
        var maskANDmaskValue = (maskValue & mask.value);

        // esto es una sola comprobación para filtrar todas las capas que no nos interesan.
        return maskANDmaskValue > 0;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("colisión con algo en la capa" + LayerMask.LayerToName(collision.gameObject.layer));

    //    // queremos que sí choque contra Enemigos (Enemy), Paredes (Wall), Obstáculos (Obstacle)
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
