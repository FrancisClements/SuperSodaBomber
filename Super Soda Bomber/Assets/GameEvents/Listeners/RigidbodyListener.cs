using UnityEngine;
/*
Void Event
    **Concrete Game Listener, from BaseGameListener

    An event listener (receiver) that returns Rigidbody2D.
    This is used for setting events using the inspector.
*/

namespace SuperSodaBomber.Events{
    public class RigidbodyListener : BaseGameListener
        <Rigidbody2D, RigidbodyEvent, UnityRigidbodyEvent>{}
}
