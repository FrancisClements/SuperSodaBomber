using UnityEngine.Events;
using UnityEngine;

/*
Void Event
    **Concrete UnityEvent
    A UnityEvent that has Rigidbody2D as a type filter.
*/

namespace SuperSodaBomber.Events{
    [System.Serializable]
    public class UnityRigidbodyEvent : UnityEvent<Rigidbody2D>{}
}