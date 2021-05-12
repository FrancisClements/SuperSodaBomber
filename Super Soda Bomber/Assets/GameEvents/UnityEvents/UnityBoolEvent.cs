using UnityEngine.Events;

/*
Void Event
    **Concrete UnityEvent
    A UnityEvent that has boolean as a type filter.
*/

namespace SuperSodaBomber.Events{
    [System.Serializable]
    public class UnityBoolEvent : UnityEvent<bool>{}
}