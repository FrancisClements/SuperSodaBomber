using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Detects the Tap Activity of the UI made by the player
/// </summary>
public class TapDetector : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// Double Click state of the UI
    /// </summary>
    public bool isDoubleClick { get; set; }

    //click data
    private float interval = .5f;
    private float firstClickTime;

    public void OnPointerDown(PointerEventData eventData){
        if(firstClickTime + interval > Time.time){
            isDoubleClick = true;
        }
        else
            isDoubleClick = false;
        firstClickTime = Time.time;
    }
}
