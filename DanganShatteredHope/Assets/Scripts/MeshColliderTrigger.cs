using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeshColliderTrigger : MonoBehaviour
{
    // List of UnityEvents to trigger once when the box collider enters the mesh collider
    public List<UnityEvent> onTriggerEnterEvents = new List<UnityEvent>();

    // List of UnityEvents to trigger once when the box collider exits the mesh collider
    public List<UnityEvent> onTriggerExitEvents = new List<UnityEvent>();

    private HashSet<Collider> collidersInside = new HashSet<Collider>(); // Track colliders inside

    private void OnTriggerEnter(Collider other)
    {
        if (other is BoxCollider && other.isTrigger)
        {
            if (!collidersInside.Contains(other))
            {
                collidersInside.Add(other);
                ExecuteEvents(onTriggerEnterEvents);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other is BoxCollider && other.isTrigger)
        {
            if (collidersInside.Contains(other))
            {
                collidersInside.Remove(other);
                ExecuteEvents(onTriggerExitEvents);
            }
        }
    }

    private void ExecuteEvents(List<UnityEvent> events)
    {
        foreach (var unityEvent in events)
        {
            unityEvent.Invoke();
        }
    }
}
