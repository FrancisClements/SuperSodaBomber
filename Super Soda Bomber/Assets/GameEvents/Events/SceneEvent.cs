using UnityEngine;

/*
Void Event
    **Concrete Game Event, from BaseGameEvent
    A sender that returns an integer.

    This will be used for creating ScriptableObjects at the Editor
    and triggers for scripts. Ex: Sending Build Indexes
*/

namespace SuperSodaBomber.Events{
    [CreateAssetMenu(fileName = "New SceneIndex Event", menuName = "GameEvents/SceneIndexEvent")]
    public class SceneEvent : BaseGameEvent<SceneIndex>{}
}
