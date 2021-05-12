/*
Void Event
    **Concrete Game Listener, from BaseGameListener

    An event listener (receiver) that returns nothing.
    This is used for setting events using the inspector.
*/

namespace SuperSodaBomber.Events{
    public class VoidListener : BaseGameListener
        <Void, VoidEvent, UnityVoidEvent>{}
}
