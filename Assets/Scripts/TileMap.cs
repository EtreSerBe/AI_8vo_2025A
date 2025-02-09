using UnityEngine;
using System.Collections.Generic;


public class TileMap : MonoBehaviour
{
    public int _width = 6;
    public int _height = 6;

    public Room[][] _roomsGrid = null;

    public Room _initialRoom = null;

    // Primero vamos a hacer el calabozo controlando el l�mite de iteraciones del algoritmo
    // _maximumIterations se refiere al m�ximo de profundidad del �rbol de pathfinding que estamos generando.
    public int _maximumIterations = 3;

    [Range(0, 1)]
    public float _newRoomProbability;

    HashSet<Room> closedList = new HashSet<Room>();
    Queue<Room> openList = new Queue<Room>();

    private bool _dungeonIsCreated = false;

    // Despu�s lo vamos a hacer controlando el n�mero de cuartos.

    void GenerateTileMap()
    {
        _roomsGrid = new Room[_height][];

        for(int j = 0; j < _height; j++)
        {
            _roomsGrid[j] = new Room[_width];
            for (int i = 0; i < _width; i++)
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

            room.iteration = parentRoom.iteration + 1;
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

    void CreateDungeon()
    {
        // Quiero que la posici�n del Room inicial sea random.
        int startX = Random.Range(0, _width);
        int startY = Random.Range(0, _height);



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
            x = currentRoom.xPos;
            y = currentRoom.yPos;

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
                TryEnqueueRoom(_roomsGrid[y - 1][x], currentRoom);
            }

            // CREAR CUARTO DERECHA
            if (x < _width - 1)  // se le pone el -1 para que el pen�ltimo a la derecha s� pueda entrar al if, pero el �ltimo no.
            {
                TryEnqueueRoom(_roomsGrid[y][x + 1], currentRoom);
            }

            // CREAR CUARTO ABAJO
            if (y < _height - 1)
            {
                TryEnqueueRoom(_roomsGrid[y + 1][x], currentRoom);
            }

            // CREAR CUARTO IZQUIERDA
            if (x > 0)  
            {
                TryEnqueueRoom(_roomsGrid[y][x - 1], currentRoom);
            }

        }


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        if (_dungeonIsCreated == false)
            return;



        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                // Lo primero ser�a instanciarlos.
                // Esto nos permite comunicar a los Rooms con el el tilemap para preguntar qu� rooms hay alrededor.
                if (_roomsGrid[j][i].occupied == true)
                {
                    Gizmos.DrawCube(new Vector3(i, j, 0), 0.5f*Vector3.one);
                }
                else
                {
                    Gizmos.DrawSphere(new Vector3(i, j, 0), 0.5f);
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(_initialRoom.xPos, _initialRoom.yPos, 0), 0.6f * Vector3.one);


    }
}
