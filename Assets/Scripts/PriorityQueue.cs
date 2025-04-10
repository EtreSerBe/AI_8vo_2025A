using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class PriorityQueue
{
    private LinkedList<EnemyStats> nodes;
    private bool lowToHigh = false;

    public PriorityQueue(bool sortLowToHigh = false)
    {
        lowToHigh = sortLowToHigh;
        nodes = new LinkedList<EnemyStats>();
    }

    public EnemyStats First()
    {
        return nodes.First.Value;
    }

    // mete al elemento dado conforme a la prioridad que tenga.
    public void Enqueue(EnemyStats node, float priority)
    {
        if (lowToHigh)
        {
            // vamos a checar todos los nodos desde el inicio, hasta encontrar uno que tenga una prioridad mayor que priority
            LinkedListNode<EnemyStats> currentNode = nodes.First;
            while (currentNode != null)
            {
                // checa cu�l es su prioridad. Si la del current node es mayor que la del que estamos tratando de insertar
                // entonces ponemos al nuevo antes que este currentNode.
                if (currentNode.Value.difficultyValue > priority)
                {
                    nodes.AddBefore(currentNode, node);
                    return;
                }

                // si la prioridad del current no fue mayor que priority, 
                // entonces pasamos currentNode al siguiente nodo.
                currentNode = currentNode.Next;
            }

            // en este punto el nodo nuevo es el menos prioritario, y por lo tanto se a�ade al final de la queue.
            nodes.AddLast(node);
        }
        else
        {
            LinkedListNode<EnemyStats> currentNode = nodes.First;
            while (currentNode != null)
            {
                // checa cu�l es su prioridad. Si la del current node es mayor que la del que estamos tratando de insertar
                // entonces ponemos al nuevo antes que este currentNode.
                if (currentNode.Value.difficultyValue < priority)
                {
                    nodes.AddBefore(currentNode, node);
                    return;
                }

                // si la prioridad del current no fue mayor que priority, 
                // entonces pasamos currentNode al siguiente nodo.
                currentNode = currentNode.Next;
            }
            // en este punto el nodo nuevo es el menos prioritario, y por lo tanto se a�ade al final de la queue.
            nodes.AddLast(node);
        }
    }

    public bool Remove(EnemyStats node)
    {
        return nodes.Remove(node);
    }

    public EnemyStats Dequeue()
    {
        EnemyStats outNode = nodes.First.Value;
        nodes.RemoveFirst();
        return outNode;
    }

    public int Count()
    {
        return nodes.Count;
    }

    // public void PrintElements()
    // {
    //     string message = string.Empty;
    //     foreach (PcgEnemy node in nodes)
    //     {
    //         message += $"X{node.}, Y{node.y} prio = {node.priority}; ";
    //     }
    //     Debug.Log(message);
    // }
}