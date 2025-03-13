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
    // radio alrededor del dueño de este script en el cual se detectarán gameObjects.
    [SerializeField]
    protected float visionRange = 12.5f;

    [SerializeField]
    protected float visionAngle = 90.0f;

    // Manera #2
    // detectar a través de colliders.
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

        Debug.Log($"el GameObject: {name} Pasó a desalerta");
        IsAlerted = false;
    }

    // Esta función se manda a llamar cuando un enemigo dentro de refDetectedEnemies
    // sale del rango o ángulo de visión.
    private IEnumerator ForgetEntity(GameObject entityToForget)
    {
        yield return new WaitForSeconds(timeToForgetEntity);

        // después de ese tiempo, olvidamos a esta entidad de refDetectedEnemies
        // eso si es que todavía estaba ahí. 
        // Primero checamos si todavía está dentro de refDetectedEnemies
        if (_refDetectedEntities.Contains(entityToForget))
        {
            Debug.Log($"GameObject: {name} está olvidando a {entityToForget.name}");
            // como sí está, entonces lo borramos de ahí.
            _refDetectedEntities.Remove(entityToForget);
        }
        // Razones por las que ya no estaría en refDetectedEnemies son, por ejemplo,
        // porque ya murió o se destruyó dicha entidad.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _visionColliderSphere = GetComponent<SphereCollider>();
        if (_visionColliderSphere != null)
        {
            _visionColliderSphere.radius = visionRange;
        }
        else // nos faltaba hacerle saber al usuario que le falta asignar esto o hubo un error en esto y debería hacer algo al respecto
        {
            Debug.LogError("visionColliderSphere es null, ¿olvidaste asignar el SphereCollider a este gameobject?");
        }
    }

    /*private void CheckVision()
    {
        
        Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, visionRange, visionLayerMask, QueryTriggerInteraction.Collide);
        // Iteramos para ver cuáles entidades ya están detectados
        foreach (Collider collider in overlappingColliders)
        {
            if(!refDetectedEnemies.Contains(collider.gameObject))
            {
                // Si no lo contiene, entonces lo empezamos a detectar
                refDetectedEnemies.Add(collider.gameObject);
            }
        }
        // Por otro lado, si está en refDetectedEnemies, pero no está en overlap sphere
        // eso significa que ya dejó de estar en rango.
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"entró a colisión con: {other.gameObject.name}");
        // si alguien choca contra nuestro visionColliderSphere,
        // entonces alguien acaba de entrar a nuestro rango de visión.

        // Si es el collider de visión, tenemos que checar la Layer de colisión.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Antes de añadirlo como detectado, necesitamos checar que esté en el cono de visión
            if(Utilities.IsInAngle(other.transform.position, transform.position, visionAngle))
            {
                // Si sí lo está, entonces lo añadimos
                AddDetectedEntity(other.gameObject);
            }
        }
    }

    protected void AddDetectedEntity(GameObject entity)
    {
        // añadimos esta entity a las entidades que están conocidas.
        _refDetectedEntities.Add(entity);
        // Si detectamos de nuevo al player,
        IsAlerted = true;
        // le decimos que cancele el desalertar si estaba activo
        if (_removeAlertCoroutine != null)
            StopCoroutine(_removeAlertCoroutine);
        // También checamos si esa entidad que chocó estaba por ser
        // olvidada, y si sí, entonces cancelamos el olvido.
        if (_entitiesToForget.ContainsKey(entity))
        {
            StopCoroutine(_entitiesToForget[entity]);
            // y después quitamos ese elemento del dictionary entitiesToForget
            _entitiesToForget.Remove(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{name} Salió de colisión con: {other.gameObject.name}");

        // si alguien deja de chocar contra nuestro visionColliderSphere,
        // entonces ya lo vamos a empezar a olvidar de nuestros entidades conocidos.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Checamos si dicha entidad está en el diccionario de entidades
            if(!_entitiesToForget.ContainsKey(other.gameObject))
            {
                // si no está en que ya se va a olvidar, entonces la comenzamos a olvidar.
                _entitiesToForget.Add(other.gameObject,
                    StartCoroutine(ForgetEntity(other.gameObject)));
            }
        }
    }

    // Función que pueden sobreescribir para modificar cómo se obtiene la 
    // entidad objetivo. Por ejemplo, la más cercana, la de menor HP, etc.
    protected virtual GameObject GetTargetEntity()
    {
        float bestDistance = float.MaxValue;
        GameObject targetNearestGameObj = null;
        foreach (GameObject obj in _refDetectedEntities)
        {
            float currentDistance = (transform.position - obj.transform.position).magnitude;
            if (currentDistance < bestDistance)
            {
                // esta es nuestra nueva mejor distancia, y guardamos a cuál Objeto se refiere.
                bestDistance = currentDistance;
                targetNearestGameObj = obj;
            }
        }

        return targetNearestGameObj;
    }

    private void FixedUpdate()
    {
        // que nos ordene los objetivos encontrados por algún parámetro, por ejemplo, la distancia de menor a mayor.
        // refEnemigosDetectados.Sort()
        _targetEntity = GetTargetEntity();
    }

    private void OnDrawGizmos()
    {
        // lo tenemos que dibujar incluso aunque aún no hayamos detectado al enemigo, 
        // para poder visualizar mejor ese radio.
        Gizmos.color = Color.green;

        if (IsAlerted)
        {        
            // Queremos saber la distancia entre el GameObject dueño de este script y el Enemigo.
            if (Utilities.IsInCone(gameObject, _targetEntity, visionRange, visionAngle))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                // si sí hay algo como Target pero no está en el cono de visión,
                // entonces ponemos el color amarillo.
                Gizmos.color = Color.yellow;
            }
        }

        // haya detectado o no al enemigo debe dibujar la esfera de detección.
        Gizmos.DrawWireSphere(transform.position, visionRange);

    }
}