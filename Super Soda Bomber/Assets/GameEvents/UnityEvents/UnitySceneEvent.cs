using UnityEngine.Events;

/*
Void Event
    **Concrete UnityEvent
    A UnityEvent that has an integer as a type filter.
*/

namespace SuperSodaBomber.Events{
    [System.Serializable]
    public class UnitySceneEvent : UnityEvent<SceneIndex>{}
}