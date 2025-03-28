using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;


public class TileMap : MonoBehaviour
{
    public byte _width = 6;
    public byte _height = 6;

    public Room[][] _roomsGrid = null;
    

    public Room _initialRoom = null;

    // Primero vamos a hacer el calabozo controlando el l�mite de iteraciones del algoritmo
    // _maximumIterations se refiere al m�ximo de profundidad del �rbol de pathfinding que estamos generando.
    public byte _maximumIterations = 3;

    [Range(0, 1)]
    public float _newRoomProbability;

    public float _straightRoomMultiplier = 2.0f;

    public float _initialProbabilityBoost = 0.5f;


    public GameObject _roomPrefab;
    public GameObject _doorPrefab;
    public GameObject _keyPrefab;

    HashSet<Room> closedList = new HashSet<Room>();
    Queue<Room> openList = new Queue<Room>();

    

    private bool _dungeonIsCreated = false;

    // Si solo obtenemos el cuarto inicial con un random entre todos los cuartos,
    // r�pidamente caer�amos en el caso de que no podemos abrir las puertas con las llaves alcanzables.
    // Por el momento lo haremos as�, pero la mejor soluci�n va a implicar checar cu�les son cuartos 
    // iniciales viables para poder abrir las puertas.
    public Room GetRandomInitialRoom()
    {
        // No podemos iterar sobre un HashSet, por lo que lo tenemos que convertir en una List antes de ello.
        List<Room> tempList = new List<Room>( closedList);
        int rand = Random.Range( 0, tempList.Count );
        // regresamos un elemento al azar.
        return tempList[rand];
    }

    // Despu�s lo vamos a hacer controlando el n�mero de cuartos.

    public enum TileDirections : byte
    {
        Up, Down, Left, Right, None
    }

    void GenerateTileMap()
    {
        _roomsGrid = new Room[_height][];

        for(byte j = 0; j < _height; j++)
        {
            _roomsGrid[j] = new Room[_width];
            for (byte i = 0; i < _width; i++)
            {
                // Lo primero ser�a instanciarlos.
                _roomsGrid[j][i] = new Room();
                // Esto nos permite comunicar a los Rooms con el el tilemap para preguntar qu� rooms hay alrededor.
                _roomsGrid[j][i].Setup(this, i, j); 
            }
        }
    }

    void TryEnqueueRoom(Room room, Room parentRoom)
    {
        if (room.occupied == false)
        {
            // si no est� ocupado, vamos si cae el caso de que s� se va a generar un cuarto aqu�.
            float result = Random.Range(0.0f, 1.0f);
            if(result > _newRoomProbability)
            {
                Debug.Log($"No se gener� el cuarto: x {room.yPos}, y {room.yPos}");
                return; // no se gener� el cuarto.
            }

            room.iteration = parentRoom.iteration;
            room.iteration++;
            room.parent = parentRoom;
            room.occupied = true;
            // si no est� ocupado ya, lo metes a la lista abierta.
            openList.Enqueue(room);

            // y luego pones puerta entre el cuarto actual y el cuarto que estamos metiendo a la lista abierta.
            // TODO:
        }
        else
        {
            Debug.Log($"el cuarto: x {room.yPos}, y {room.yPos} est� ocupado");
        }
    }

    bool TryEnqueueRoomPreferStraightLines(Room room, Room parentRoom, TileDirections enqueuingDirection)
    {
        if (room.occupied == false)
        {
            // que la probabilidad base de crear cuarto sea m�s alta entre menor sea la iteraci�n actual.
            var currentProbabilityBoost = 
                (_initialProbabilityBoost / _maximumIterations) * (_maximumIterations - parentRoom.iteration);


            // si el enqueuingDirection es == parentRoom.parentDirection,
            // entonces la probabilidad de generar cuarto se vuelve m�s alta
            var roomProbability = _newRoomProbability;
            if (parentRoom.parentDirection == enqueuingDirection)
                roomProbability *= _straightRoomMultiplier;

            // lo hago despu�s de la multiplicaci�n de arriba para que no tome el boost al momento de dar preferencia
            // a los cuartos en l�nea recta.
            roomProbability += currentProbabilityBoost;

            // si no est� ocupado, vamos si cae el caso de que s� se va a generar un cuarto aqu�.
            float result = Random.Range(0.0f, 1.0f);
            if (result > roomProbability)
            {
                Debug.Log($"No se gener� el cuarto: x {room.yPos}, y {room.yPos}");
                // m�rcalo como ocupado aunque no se haya encolado, para evitar que se intente generar este cuarto varias veces.
                // room.parent = parentRoom; // solo para motivos de debug draw.
                room.occupied = true;
                room.walkable = false; 
                return false; // no se gener� el cuarto.
            }

            room.iteration = parentRoom.iteration;
            room.iteration++; 
            room.parent = parentRoom;
            room.occupied = true;
            room.parentDirection = enqueuingDirection;
            // A�adimos la puerta entre estos dos cuartos, se la a�adimos a cada uno.
            Door newDoor = new Door(room, parentRoom);
            //float enqueueHorizontalOffset = enqueuingDirection == TileDirections.Left ? -1.0f :
            //    enqueuingDirection == TileDirections.Right ? 1.0f : 0.0f;
            //float enqueueVerticalOffset = enqueuingDirection == TileDirections.Down ? -1.0f :
            //    enqueuingDirection == TileDirections.Up ? 1.0f : 0.0f;

            // Instantiate(_doorPrefab, new Vector3(parentRoom.xPos, 0.0f, parentRoom.yPos - 0.5f), Quaternion.identity, gameObject.transform);

            room.AddDoor(newDoor);
            parentRoom.AddDoor(newDoor);

            // si no est� ocupado ya, lo metes a la lista abierta.
            openList.Enqueue(room);

            // y luego pones puerta entre el cuarto actual y el cuarto que estamos metiendo a la lista abierta.
            // TODO:
            return true;
        }
        else
        {
            // Debug.Log($"el cuarto: x {room.yPos}, y {room.yPos} est� ocupado");
        }
        return false;
    }

    void CreateDungeon()
    {
        // Quiero que la posici�n del Room inicial sea random.
        // int startX = Random.Range(0, _width);
        // int startY = Random.Range(0, _height);

        // Ahorita quiere que el inicio sea en el mero centro de la cuadr�cula.
        int startX = _width/2;
        int startY = _height/2;


        // En ese cuarto ya podemos comenzar el proceso de generaci�n.
        _initialRoom = _roomsGrid[startY][startX];

        // Metemos el elemento inicial a la lista abierta
        _initialRoom.parent = null;
        _initialRoom.occupied = true;
        // si no est� ocupado ya, lo metes a la lista abierta.
        openList.Enqueue(_initialRoom);

        Room currentRoom = null;

        int x = 0;
        int y = 0;

        while (openList.Count > 0)
        {
            currentRoom = openList.Dequeue();
            // cuando sacas a alguien de la lista abierta aqu�, lo tienes que meter a la lista cerrada
            closedList.Add(currentRoom); 
            x = currentRoom.xPos;
            y = currentRoom.yPos;

            // spawneamos un gameobject de cuarto en la posici�n del current room.
            GameObject currentSpawnedRoom = Instantiate(_roomPrefab, new Vector3(x, 0.0f, y), Quaternion.identity, this.transform);

            // Si ya llegamos al n�mero m�ximo de iteraciones, dejamos de generar m�s cuartos (de conectarlos).
            if(currentRoom.iteration >= _maximumIterations) 
            {
                Debug.Log("llegamos al m�ximo de iteraciones");
                break; // nos salimos del ciclo.
            }

            // Al nodo que estamos expandiendo actualmente, le vamos a preguntar si va a generar los cuartos adyacentes.
            // CREAR CUARTO ARRIBA
            if (currentRoom.yPos > 0)
            {
                // Si es mayor que 0 s� podemos intentar generar cuarto arriba.
                // si no, entonces NO podemos crear un cuarto arriba, porque 0-1 = -1, y ser�a pedirle a un array el elemento [-1]
                // esta comprobaci�n es para evitar causar access violations.
                // TryEnqueueRoom(_roomsGrid[y - 1][x], currentRoom);
                if(TryEnqueueRoomPreferStraightLines(_roomsGrid[y - 1][x], currentRoom, TileDirections.Up))
                {
                    // si s� se a�adi� un cuarto, entonces podemos generar una puerta.
                    Instantiate(_doorPrefab, new Vector3(x, 0.0f, y - 0.5f), Quaternion.identity, currentSpawnedRoom.transform);
                }
            }

            // CREAR CUARTO DERECHA
            if (x < _width - 1)  // se le pone el -1 para que el pen�ltimo a la derecha s� pueda entrar al if, pero el �ltimo no.
            {
                // TryEnqueueRoom(_roomsGrid[y][x + 1], currentRoom);
                if(TryEnqueueRoomPreferStraightLines(_roomsGrid[y][x + 1], currentRoom, TileDirections.Right))
                {
                    // si s� se a�adi� un cuarto, entonces podemos generar una puerta.
                    GameObject spawnedDoor = Instantiate(_doorPrefab, new Vector3(x + 0.5f, 0.0f, y), Quaternion.identity, currentSpawnedRoom.transform);
                    spawnedDoor.transform.Rotate(0, 90, 0);
                }
            }

            // CREAR CUARTO ABAJO
            if (y < _height - 1)
            {
                // TryEnqueueRoom(_roomsGrid[y + 1][x], currentRoom);
                if(TryEnqueueRoomPreferStraightLines(_roomsGrid[y + 1][x], currentRoom, TileDirections.Down))
                {
                    // si s� se a�adi� un cuarto, entonces podemos generar una puerta.
                    Instantiate(_doorPrefab, new Vector3(x, 0.0f, y + 0.5f), Quaternion.identity, currentSpawnedRoom.transform);
                }
            }

            // CREAR CUARTO IZQUIERDA
            if (x > 0)  
            {
                // TryEnqueueRoom(_roomsGrid[y][x - 1], currentRoom);
                if(TryEnqueueRoomPreferStraightLines(_roomsGrid[y][x - 1], currentRoom, TileDirections.Left))
                {
                    // si s� se a�adi� un cuarto, entonces podemos generar una puerta.
                    GameObject spawnedDoor = Instantiate(_doorPrefab, new Vector3(x - 0.5f, 0.0f, y), Quaternion.identity, currentSpawnedRoom.transform);
                    spawnedDoor.transform.Rotate(0, 90, 0);
                }
            }

        }


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize(); // lo coment� porque ahora se va a mandar a llamar desde el Start del DungeonGameManager.

        //Renderer exampleRenderer = GetComponent<Renderer>();
        //if (exampleRenderer == null) 
        //{
        //    Debug.LogError("el componente Renderer para exampleRenderer no se encontr�");
        //}

        // operaci�n X
        // checar resultado de operaci�n X
        // si fall� la operaci�n, detener la ejecuci�n o hacer que truene lo menos posible
        // o reintentar la operaci�n, etc. seg�n sea el caso.
    }

    public void Initialize()
    {
        GenerateTileMap();
        CreateDungeon();
        _dungeonIsCreated = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        /*if (_dungeonIsCreated == false)
            return;



        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                Gizmos.color = Color.white;

                // Lo primero ser�a instanciarlos.
                // Esto nos permite comunicar a los Rooms con el el tilemap para preguntar qu� rooms hay alrededor.
                if (_roomsGrid[j][i].occupied == true)
                {
                    Vector3 nodePos = new Vector3(i, j, 0);
                    if (_roomsGrid[j][i].parent == null)
                    {
                        // entonces se visit� pero no se cre� este cuarto
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(new Vector3(i, j, 0), 0.25f * Vector3.one);
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawCube(new Vector3(i, j, 0), 0.5f * Vector3.one);
                    }
                    // dibujamos una l�nea entre el parent y este nodo.
                    if(_roomsGrid[j][i].parent != null)
                    {
                        Vector3 parentPos = new Vector3(_roomsGrid[j][i].parent.xPos, _roomsGrid[j][i].parent.yPos, 0);
                        Gizmos.DrawLine(nodePos, parentPos);
                    }    
                }
                else
                {
                    Gizmos.DrawSphere(new Vector3(i, j, 0), 0.5f);
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(_initialRoom.xPos, _initialRoom.yPos, 0), 0.6f * Vector3.one);
        */

    }
}
