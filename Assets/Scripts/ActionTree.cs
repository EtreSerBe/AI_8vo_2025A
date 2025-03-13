using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// los nodos de este �rbol tienen que poder representar todo el estado del mundo en ese instante.
// representar las cosas relevantes al proceso espec�fico para el que se est� usando este �rbol.
public class DungeonStateNode
{
    public DungeonStateNode()
    {

    }

    public DungeonStateNode(ref TileMap map)
    {
        _width = map._width;
        _height = map._height;
        _roomsGrid = map._roomsGrid;
        _initialRoom = map._initialRoom;
    }

    // devuelve un nodo con exactamente los mismos valores PERO es un objeto separado.
    public static DungeonStateNode Copy(DungeonStateNode original)
    {
        DungeonStateNode dungeonStateNode = new DungeonStateNode();
        dungeonStateNode._width = original._width;
        dungeonStateNode._height = original._height;
        dungeonStateNode._roomsGrid = original._roomsGrid;
        dungeonStateNode._initialRoom = original._initialRoom;
        dungeonStateNode.currentPlayerObtainedKeys = original.currentPlayerObtainedKeys;
        dungeonStateNode.lockedDoors = original.lockedDoors;
        dungeonStateNode.lockTypesInUse = original.lockTypesInUse;
        return dungeonStateNode;
    }

    // Estas variables nos bastan para describir el estado del mundo seg�n lo necesitemos.
    public byte _width;
    public byte _height; // uso byte porque solamente ocupan 1 byte en memoria, y porque nuestra grid no va a ser tan grande.

    // 81 cuartos * 44 bytes cada uno = 3564 bytes

    // internamente cada cuarto tiene la referencia a sus (hasta) 4 puertas y sabe si tiene power up o no.
    public Room[][] _roomsGrid = null;  
    // Este room inicial debe de ser D�nde est� parado el jugador en este momento en el mundo.
    public Room _initialRoom = null;

    // Las llaves que tiene actualmente el jugador. TODO: Cambiar esto por algo m�s apropiado.
    public HashSet<byte> currentPlayerObtainedKeys = new HashSet<byte>();



    // Necesario para moverse a trav�s del �rbol del proceso de generaci�n del dungeon.
    public DungeonStateNode _parent = null;

    public HashSet<Door> lockedDoors = new HashSet<Door>();

    public HashSet<byte> lockTypesInUse = new HashSet<byte>();

    // funci�n que nos dice cu�les llaves quedan sin usarse.
    // De todas las llaves existentes, cu�les tipos quedan sin ponerse en el mapa.
    public List<byte> GetUnusedKeyTypes()
    {
        List<byte> result = new List<byte>();
        for(byte i = 0; i < ((byte)DungeonKeys.NUMBER_OF_KEYS); i++)
        { 
            // Si este tipo de cerrojo no est� en uso a�n, entonces s� lo queremos regresar en la lista.
            if(!lockTypesInUse.Contains(i))
            {
                result.Add(i);
            }
        }

        // regresamos una lista con todos los que no se han usado.
        return result;
    }
}




public class ActionTree : MonoBehaviour
{
    // Un �rbol est� compuesto por nodos.

    // un �rbol tiene una ra�z (Node ra�z).

    // qu� profundidad tiene este �rbol.


    // Poder checar a cu�les Llaves se puede acceder en el mundo y en qu� orden.
    // Cuando estemos checando el estado del calabozo en cierto nodo, 
    // vamos a hacer una serie de pathfindings.
    // los primeros pathfindings van a ser las llaves que hay disponibles desde la ubicaci�n actual del jugador.
    // despu�s de tener las llaves, vamos a hacer pathfinding de cada llave hacia cada puerta que corresponda a esa llave
    TileMap _tileMap;

    DungeonStateNode _initialState;
    DungeonStateNode _currentState;


    void TryEnqueueReachableRoom(Room enqueuedRoom, Room currentNode, 
        ref Queue<Room> openNodes, ref HashSet<Room> closedNodes,  
        ref DungeonStateNode currentDungeonState)
    {
        // Si ya est� en la lista cerrada, no hay que encolarlo.
        if (closedNodes.Contains(enqueuedRoom))
            return;
        // Quit� esto porque s� deber�an tener padre ya, se le asign� en la generaci�n del calabozo.
        /*&& enqueuedRoom.parent == null*/
        if (enqueuedRoom.walkable)
        {
            // checar si s� hay puerta conectando a estos dos cuartos.
            Door connectingDoor = enqueuedRoom.GetDoorToRoom(currentNode);
            if (connectingDoor == null)
            {
                // Si no hay puerta, entonces nos salimos de esta funci�n.
                return;
            }
            // si s� hay puerta, checamos si s� est� cerrada o no.
            if (connectingDoor._keyId == 0 || currentDungeonState.currentPlayerObtainedKeys.Contains(
                connectingDoor._keyId))
            {
                enqueuedRoom.parent = currentNode;
                openNodes.Enqueue(enqueuedRoom);
            }
            // si no tenemos la llave y s� est� cerrada, pues no es alcanzable.
        }
    }

    // funci�n para determinar cu�les son puntos/rooms v�lidos y alcanzables dentro del mapa.
    private List<Room> GetReachableRooms(ref DungeonStateNode currentDungeonState)
    {
        List<Room> result = new List<Room>();

        // metemos a esa lista todos los nodos que podamos alcanzar con un algoritmo de pathfinding el que sea.
        // Por ejemplo, un breadth first search.
        // Player state: qu� llaves/habilides tiene, d�nde est� en el mapa
        Queue<Room> openNodes = new Queue<Room>();
        // Necesario para evitar re-encolar Rooms que ya se hayan marcado como alcanzables.
        HashSet<Room> closedNodes = new HashSet<Room>();

        // �l es su propio padre, para que no lo intenten meter al pathfinding otra vez.
        currentDungeonState._initialRoom.parent = currentDungeonState._initialRoom;
        // el nodo inicial es donde est� nuestro personaje en este momento
        openNodes.Enqueue(currentDungeonState._initialRoom);

        Room currentNode = null;

        // WARNING: Si el n�mero de nodos fuera MUY grande, lo ideal ser�a limitar cu�ntos nodos queremos en result.
        // de lo contrario, se gastar�a much�simos recursos en eso.
        while (openNodes.Count > 0)
        {
            // metemos todos los nodos que podamos.
            currentNode = openNodes.Dequeue();
            closedNodes.Add(currentNode);
            result.Add(currentNode);

            byte x = currentNode.xPos;
            byte y = currentNode.yPos;
            // Vecino Arriba
            if(y < currentDungeonState._height -1)
            {
                Room enqueuedRoom = currentDungeonState._roomsGrid[y + 1][x];
                TryEnqueueReachableRoom(enqueuedRoom, currentNode, ref openNodes, ref closedNodes,
                    ref currentDungeonState);
            }

            // Vecino Derecha
            if (x < currentDungeonState._width - 1)
            {
                Room enqueuedRoom = currentDungeonState._roomsGrid[y][x + 1];
                TryEnqueueReachableRoom(enqueuedRoom, currentNode, ref openNodes, ref closedNodes,
                    ref currentDungeonState);
            }

            // Vecino Abajo
            if (y > 0)
            {
                Room enqueuedRoom = currentDungeonState._roomsGrid[y - 1][x];
                TryEnqueueReachableRoom(enqueuedRoom, currentNode, ref openNodes, ref closedNodes,
                    ref currentDungeonState);
            }

            // Vecino Izquierda
            if (x > 0)
            {
                Room enqueuedRoom = currentDungeonState._roomsGrid[y][x - 1];
                TryEnqueueReachableRoom(enqueuedRoom, currentNode, ref openNodes, ref closedNodes, 
                    ref currentDungeonState);
            }
        }


        return result;
    }

    private List<Room> GetLockableRooms(ref List<Room> reachableRooms)
    {
        List<Room> result = new List<Room>();

        // Filtramos a los que ya tienen todas sus puertas con cerrojos.
        foreach (Room room in reachableRooms)
        {
            if(room.GetDoorsWithNoLockAssigned().Count > 0)
            {
                // Si s� tiene alguna puerta que no tenga ninguna llave asignada, entonces s� es cerrable.
                result.Add(room);
            }
            // si no, la ignoramos y ya.
        }

        return result;
    }



    void KeyAndLockGeneration(TileMap initialDungeonState)
    {
        DungeonStateNode currentState = new DungeonStateNode(ref initialDungeonState);
        // 1) Poner al personaje en un punto v�lido del mapa.
        // para ponerlo en un punto v�lido, tenemos que obtener todos los puntos v�lidos del mapa y de ah� elegir uno.
        List<Room> initialReachableRooms = GetReachableRooms(ref currentState);
        // ahora s� seleccionamos uno al azar.
        int rand = Random.Range(0, initialReachableRooms.Count);
        Room initialRoom = initialReachableRooms[rand]; // aqu� es donde va a aparecer el jugador inicialmente.



        // 2) obtener todos los lugares alcanzables desde donde est� el jugador en ese momento.
        // Son initialReachableRooms.


        // 3) Si hay un n�mero de llaves igual al de puertas (por ID),
        // poner una puerta en uno de esos lugares alcanzables.
        if(currentState.lockedDoors.Count == currentState.currentPlayerObtainedKeys.Count)
        {
            // Creamos un nuevo dungeon state pero con esta nueva puerta
            DungeonStateNode newState = DungeonStateNode.Copy(currentState);
            // Necesitamos obtener el ID de la llave que se usar� para cerrar esta puerta.
            // random de las llaves disponibles sin usar 
            List<byte> keyType = newState.GetUnusedKeyTypes();
            // hacemos un random para elegir una de estos posibles tipos de llaves.
            rand = Random.Range(0, keyType.Count);
            byte keyTypeToUse = keyType[rand];

            List<Room> lockableRooms = GetLockableRooms(ref initialReachableRooms);

            rand = Random.Range(0, lockableRooms.Count); // s� puede incluir a donde est� el player actualmente
            Room roomToLock = lockableRooms[rand]; // aqu� es donde va a aparecer el jugador inicialmente.

            roomToLock = newState._roomsGrid[roomToLock.yPos][roomToLock.xPos];

            // Obtenemos todas las puertas sin cerrar de ese cuarto
            List<Door> lockableDoors = roomToLock.GetDoorsWithNoLockAssigned();
            rand = Random.Range(0, lockableDoors.Count);
            Door doorToLock = lockableDoors[rand];
            
            // TO DO: Checar de qu� lado debe ir la puerta.
            Instantiate(_tileMap._doorPrefab, new Vector3(doorToLock.sideA.xPos, 1.0f, doorToLock.sideA.yPos), Quaternion.identity, gameObject.transform);

            Debug.Log($"Colocando puerta cerrada en el cuarto: X{roomToLock.xPos}, Y{roomToLock.yPos} en la puerta" +
                $"que va X{doorToLock.sideA.xPos}, Y{doorToLock.sideA.yPos} hacia X{doorToLock.sideB.xPos}, Y{doorToLock.sideB.yPos}");


            // entonces ponemos una puerta nueva cerrada con este tipo de cerrojo elegido de los disponibles.
            doorToLock.LockDoor(keyTypeToUse);

            // A�adimos la puerta cerrada a este nuevo estado de mundo.
            newState.lockedDoors.Add(doorToLock);

            // Le decimos que ya tiene este tipo de llave en uso.
            newState.lockTypesInUse.Add(keyTypeToUse);

            // finalmente metemos este nuevo nodo del estado del calabozo a
            // nuestro DepthFirstSearch de posibles acciones/estados de mundo. (vea paso '5)' )

            // NOTA: Ahorita esto es a cada llave corresponde una puerta
        }
        else 
        {
            // 3.5) si hay m�s puertas, poner una llave que le corresponda a la puerta sin llave,
            // en un lugar alcanzable.
            // Creamos un nuevo dungeon state pero con esta nueva llave
            DungeonStateNode newState = DungeonStateNode.Copy(currentState);

            byte missingLockType = 0;
            // vamos a poner el tipo de llave que s� est� en uso pero el player todav�a no obtenga.
            foreach(byte lockType in newState.lockTypesInUse)
            {
                if(!newState.currentPlayerObtainedKeys.Contains(lockType))
                {
                    // si no lo contiene, entonces este es el que hay que poner en el mapa.
                    missingLockType = lockType;
                    break; // nos salimos de este foreach.
                }
            }

            // cuarto donde vamos a spawnear la llave.
            rand = Random.Range(0, initialReachableRooms.Count); // s� puede incluir a donde est� el player actualmente
            Room roomToLock = initialReachableRooms[rand]; // aqu� es donde va a aparecer el jugador inicialmente.

            roomToLock = newState._roomsGrid[roomToLock.yPos][roomToLock.xPos];
            // le decimos que spawnee la llave ah�.

            // por simplicidad le voy a decir que el jugador la "tiene" en cuanto la spawneamos.
            // NOTA: lo ideal ser�a mover al player a la posici�n de esta llave y de ah� seguir el proceso. Lo haremos m�s tarde
            newState.currentPlayerObtainedKeys.Add(missingLockType);
        }

        // 4) el paso 3 o 3.5 nos genera un nuevo DungeonStateNode que tiene ya esta puerta o llave.
        // 5) cambiar de estado hacia ese nuevo DungeonStateNode, y repetir desde 1).
        // NOTA, a lo mejor nos falt� el punto de mover al personaje, pero ahorita vemos qu� tal sale.

        // Cu�l es nuestra condici�n de terminaci�n? cu�ndo detenemos este algoritmo?
        // por ejemplo: al poner 3 pares de llaves y puertas?
        // al explorar 20 posibles nodos?
        // qu� pasa si ya no hay m�s posibles acciones? cu�ndo sucede �sto? En el Sudoku, cu�ndo pasar�a esto?
    }


    // Regla nuestra: No importa c�mo est� el calabozo, no puede haber ninguna puerta que no se pueda abrir.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tileMap = GetComponent<TileMap>();
        _initialState = new DungeonStateNode(ref _tileMap);
        _currentState = _initialState;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            KeyAndLockGeneration(_tileMap);
        }
    }
}
