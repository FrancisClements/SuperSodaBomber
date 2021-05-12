using UnityEngine;

/*
Void Event
    **Concrete Game Event, from BaseGameEvent
    A sender that returns a Rigidbody2D.

    This will be used for creating ScriptableObjects at the Editor
    and triggers for scripts.
*/

namespace SuperSodaBomber.Events{
    [CreateAssetMenu(fileName = "New Rigidbody Event", menuName = "GameEvents/RigidbodyEvent")]
    public class RigidbodyEvent : BaseGameEvent<Rigidbody2D>{}
}
