using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Senses : MonoBehaviour
{

    protected HashSet<GameObject> _refDetectedEntities = new HashSet<GameObject>();

    protected Dictionary<GameObject, Coroutine> _entitiesToForget = new Dictionary<GameObject, Coroutine>();

    // protected MonoBehaviour 
    [SerializeField] 
    protected LayerMask visionLayerMask;

    // Manera #1
    // radio alrededor del due�o de este script en el cual se detectar�n gameObjects.
    [SerializeField]
    protected float visionRange = 12.5f;

    [SerializeField]
    protected float visionAngle = 90.0f;

    // Manera #2
    // detectar a trav�s de colliders.
    protected SphereCollider _visionColliderSphere;

    [SerializeField]
    protected float timeToRemoveAlert = 5.0f;

    [SerializeField]
    protected float timeToForgetEntity = 5.0f;

    protected GameObject _targetEntity = null;

    private Coroutine _removeAlertCoroutine;

    bool IsAlerted = false;

    public GameObject GetTargetEnemy()
    {
        return _targetEntity;
    }

    public bool IsEnemyDetected() { return _targetEntity != null; }



    private IEnumerator RemoveAlert()
    {
        yield return new WaitForSeconds(timeToRemoveAlert);

        Debug.Log($"el GameObject: {name} Pas� a desalerta");
        IsAlerted = false;
    }

    // Esta funci�n se manda a llamar cuando un enemigo dentro de refDetectedEnemies
    // sale del rango o �ngulo de visi�n.
    private IEnumerator ForgetEntity(GameObject entityToForget)
    {
        yield return new WaitForSeconds(timeToForgetEntity);

        // despu�s de ese tiempo, olvidamos a esta entidad de refDetectedEnemies
        // eso si es que todav�a estaba ah�. 
        // Primero checamos si todav�a est� dentro de refDetectedEnemies
        if (_refDetectedEntities.Contains(entityToForget))
        {
            Debug.Log($"GameObject: {name} est� olvidando a {entityToForget.name}");
            // como s� est�, entonces lo borramos de ah�.
            _refDetectedEntities.Remove(entityToForget);
        }
        // Razones por las que ya no estar�a en refDetectedEnemies son, por ejemplo,
        // porque ya muri� o se destruy� dicha entidad.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _visionColliderSphere = GetComponent<SphereCollider>();
        if (_visionColliderSphere != null)
        {
            _visionColliderSphere.radius = visionRange;
        }
        else // nos faltaba hacerle saber al usuario que le falta asignar esto o hubo un error en esto y deber�a hacer algo al respecto
        {
            Debug.LogError("visionColliderSphere es null, �olvidaste asignar el SphereCollider a este gameobject?");
        }
    }

    /*private void CheckVision()
    {
        
        Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, visionRange, visionLayerMask, QueryTriggerInteraction.Collide);
        // Iteramos para ver cu�les entidades ya est�n detectados
        foreach (Collider collider in overlappingColliders)
        {
            if(!refDetectedEnemies.Contains(collider.gameObject))
            {
                // Si no lo contiene, entonces lo empezamos a detectar
                refDetectedEnemies.Add(collider.gameObject);
            }
        }
        // Por otro lado, si est� en refDetectedEnemies, pero no est� en overlap sphere
        // eso significa que ya dej� de estar en rango.
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"entr� a colisi�n con: {other.gameObject.name}");
        // si alguien choca contra nuestro visionColliderSphere,
        // entonces alguien acaba de entrar a nuestro rango de visi�n.

        // Si es el collider de visi�n, tenemos que checar la Layer de colisi�n.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Antes de a�adirlo como detectado, necesitamos checar que est� en el cono de visi�n
            if(Utilities.IsInAngle(other.transform.position, transform.position, visionAngle))
            {
                // Si s� lo est�, entonces lo a�adimos
                AddDetectedEntity(other.gameObject);
            }
        }
    }

    protected void AddDetectedEntity(GameObject entity)
    {
        // a�adimos esta entity a las entidades que est�n conocidas.
        _refDetectedEntities.Add(entity);
        // Si detectamos de nuevo al player,
        IsAlerted = true;
        // le decimos que cancele el desalertar si estaba activo
        if (_removeAlertCoroutine != null)
            StopCoroutine(_removeAlertCoroutine);
        // Tambi�n checamos si esa entidad que choc� estaba por ser
        // olvidada, y si s�, entonces cancelamos el olvido.
        if (_entitiesToForget.ContainsKey(entity))
        {
            StopCoroutine(_entitiesToForget[entity]);
            // y despu�s quitamos ese elemento del dictionary entitiesToForget
            _entitiesToForget.Remove(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{name} Sali� de colisi�n con: {other.gameObject.name}");

        // si alguien deja de chocar contra nuestro visionColliderSphere,
        // entonces ya lo vamos a empezar a olvidar de nuestros entidades conocidos.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Checamos si dicha entidad est� en el diccionario de entidades
            if(!_entitiesToForget.ContainsKey(other.gameObject))
            {
                // si no est� en que ya se va a olvidar, entonces la comenzamos a olvidar.
                _entitiesToForget.Add(other.gameObject,
                    StartCoroutine(ForgetEntity(other.gameObject)));
            }
        }
    }

    // Funci�n que pueden sobreescribir para modificar c�mo se obtiene la 
    // entidad objetivo. Por ejemplo, la m�s cercana, la de menor HP, etc.
    protected virtual GameObject GetTargetEntity()
    {
        float bestDistance = float.MaxValue;
        GameObject targetNearestGameObj = null;
        foreach (GameObject obj in _refDetectedEntities)
        {
            float currentDistance = (transform.position - obj.transform.position).magnitude;
            if (currentDistance < bestDistance)
            {
                // esta es nuestra nueva mejor distancia, y guardamos a cu�l Objeto se refiere.
                bestDistance = currentDistance;
                targetNearestGameObj = obj;
            }
        }

        return targetNearestGameObj;
    }

    private void FixedUpdate()
    {
        // que nos ordene los objetivos encontrados por alg�n par�metro, por ejemplo, la distancia de menor a mayor.
        // refEnemigosDetectados.Sort()
        _targetEntity = GetTargetEntity();
    }

    private void OnDrawGizmos()
    {
        // lo tenemos que dibujar incluso aunque a�n no hayamos detectado al enemigo, 
        // para poder visualizar mejor ese radio.
        Gizmos.color = Color.green;

        if (IsAlerted)
        {        
            // Queremos saber la distancia entre el GameObject due�o de este script y el Enemigo.
            if (Utilities.IsInCone(gameObject, _targetEntity, visionRange, visionAngle))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                // si s� hay algo como Target pero no est� en el cono de visi�n,
                // entonces ponemos el color amarillo.
                Gizmos.color = Color.yellow;
            }
        }

        // haya detectado o no al enemigo debe dibujar la esfera de detecci�n.
        Gizmos.DrawWireSphere(transform.position, visionRange);

    }
}