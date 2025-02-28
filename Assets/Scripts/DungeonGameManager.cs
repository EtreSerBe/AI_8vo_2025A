using NUnit.Framework;
using UnityEngine;

public class DungeonGameManager : MonoBehaviour
{

    // Prefab al player que se va a spawnear en la posición inicial del grid.
    [SerializeField]
    private PlayerCharacterController _playerCharacterController;



    // referencia al script de TileMap que genera el calabozo. Debe de estar seteado en escena en este gameObject.
    private TileMap _tileMap;

    private Room _initialRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!TryGetComponent<TileMap>(out _tileMap))
        {
            Debug.LogError($"Este gameObject debería tener un script de TileMap, pero no lo tiene");
            return;
        }
        _tileMap.Initialize(); // creamos el calabozo.

        // Creación de puertas y llaves.
        // Primer paso determinar el inicio del jugador.
        _initialRoom = _tileMap.GetRandomInitialRoom();

        // Después, es poner una puerta en desde el inicio del calabozo.
        // el poner la puerta nos debe actualizar qué cuartos sí son alcanzables y cuáles ya no.
        // Como estamos usando un grafo en el que hay un solo camino a cada nodo, 
        // cortar una arista nos limita todos los nodos que estén conectado a él del otro lado.
        //List<Room> reachableRooms = ; // esta nos la da el pathfinding que se implementó

        // tomamos un cuarto de esos al azar.
        // int rand = Random.Range(0, reachableRooms.Count);

        // hay que cortarle la conexión del lado de quien fue su parent en esa ejecución del algoritmo.
        // en este momento sabemos que las puertas representan aristar en nuestro calabozo.
        // entonces, durante nuestro algoritmo de pathfinding de esto específicamente vamos a checar
        // (además de arriba, abajo, izquierda, derecha), pues que la puerta entre ellos dos esté abierta.
        // si no lo está, pues no podemos encolarlo en el algoritmo de pathfinding.

        // NOTA: Vamos a usar llaves y puertas NO-genéricas, es decir, una llave X solo abre la puerta X,
        // no una puerta Y.

        // IMPORTANTE: Ahorita nuestro algoritmo está limitado en que una vez que se pone una puerta cerrada,
        // todo lo que se quede "cerrado" detrás de ella, ya no está siendo considerado para la generación 
        // de futuras llaves y puertas.

        // una vez que se creó, podemos colocar al player en un espacio válido que se haya creado.

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
