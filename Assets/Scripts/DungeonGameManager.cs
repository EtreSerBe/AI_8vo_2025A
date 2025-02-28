using NUnit.Framework;
using UnityEngine;

public class DungeonGameManager : MonoBehaviour
{

    // Prefab al player que se va a spawnear en la posici�n inicial del grid.
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
            Debug.LogError($"Este gameObject deber�a tener un script de TileMap, pero no lo tiene");
            return;
        }
        _tileMap.Initialize(); // creamos el calabozo.

        // Creaci�n de puertas y llaves.
        // Primer paso determinar el inicio del jugador.
        _initialRoom = _tileMap.GetRandomInitialRoom();

        // Despu�s, es poner una puerta en desde el inicio del calabozo.
        // el poner la puerta nos debe actualizar qu� cuartos s� son alcanzables y cu�les ya no.
        // Como estamos usando un grafo en el que hay un solo camino a cada nodo, 
        // cortar una arista nos limita todos los nodos que est�n conectado a �l del otro lado.
        //List<Room> reachableRooms = ; // esta nos la da el pathfinding que se implement�

        // tomamos un cuarto de esos al azar.
        // int rand = Random.Range(0, reachableRooms.Count);

        // hay que cortarle la conexi�n del lado de quien fue su parent en esa ejecuci�n del algoritmo.
        // en este momento sabemos que las puertas representan aristar en nuestro calabozo.
        // entonces, durante nuestro algoritmo de pathfinding de esto espec�ficamente vamos a checar
        // (adem�s de arriba, abajo, izquierda, derecha), pues que la puerta entre ellos dos est� abierta.
        // si no lo est�, pues no podemos encolarlo en el algoritmo de pathfinding.

        // NOTA: Vamos a usar llaves y puertas NO-gen�ricas, es decir, una llave X solo abre la puerta X,
        // no una puerta Y.

        // IMPORTANTE: Ahorita nuestro algoritmo est� limitado en que una vez que se pone una puerta cerrada,
        // todo lo que se quede "cerrado" detr�s de ella, ya no est� siendo considerado para la generaci�n 
        // de futuras llaves y puertas.

        // una vez que se cre�, podemos colocar al player en un espacio v�lido que se haya creado.

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
