using UnityEngine.Events;

/*
Void Event
    **Concrete UnityEvent
    A UnityEvent that has Void.cs as a type filter.
*/

namespace SuperSodaBomber.Events{
    [System.Serializable]
    public class UnityVoidEvent : UnityEvent<Void>{}
}