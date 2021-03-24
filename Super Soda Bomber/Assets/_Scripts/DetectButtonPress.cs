using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*Detect Button Press
	Used for buttons that want to detect whether the player
	pressed down/released the button

	Mainly used for jump-anticipation animation of the player
*/

//IPointerDownHandler detects the pressing down of the button, vice versa on IPointerUpHandler
public class DetectButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private bool isPressed;
	public void OnPointerDown(PointerEventData eventData){
		isPressed = true;
	}
 
	public void OnPointerUp(PointerEventData eventData){
		isPressed = false;
	}

	public bool getPressedStatus(){
		return isPressed;
	}
}
