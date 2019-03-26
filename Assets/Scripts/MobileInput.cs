using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour {
	private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
	private Vector2 swipeDelta, startTouch;
	public static MobileInput Instance { set; get; }
	private const float DEAD_ZONE = 50.0f;

	public bool Tap{ get { return tap; } }
	public Vector3 SwipeDelta { get { return swipeDelta; } }
	public bool SwipeLeft { get { return swipeLeft; } }
	public bool SwipeRight { get { return swipeRight; } }
	public bool SwipeUp { get { return swipeUp; } }
	public bool SwipeDown { get { return swipeDown; } }

	private void Awake(){
		Instance = this;
	}

	private void Update(){
		//reseting all the booleans
		tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

		#region Standalone Inputs
		//lets check for inputs
		if (Input.GetMouseButtonDown (0)) {
			tap = true;
			startTouch = Input.mousePosition;
		} else if(Input.GetMouseButtonUp(0)){
			startTouch = swipeDelta = Vector2.zero;
		}
		#endregion

		#region Mobile Inputs
		//lets check for inputs
		if (Input.touches.Length !=0) {
			if(Input.touches[0].phase == TouchPhase.Began){
				tap = true;
				startTouch = Input.mousePosition;
			}
			else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled){
				startTouch = swipeDelta = Vector2.zero;
			}
		} 
		#endregion

		//calculate distance
		swipeDelta = Vector2.zero;
		if (startTouch != Vector2.zero) {
			if (Input.touches.Length != 0) {
				swipeDelta = Input.touches [0].position - startTouch;
			} 
			else if (Input.GetMouseButton(0)) {
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
			}
		}

		//lets check is were beyond the deadzone
		if (swipeDelta.magnitude > DEAD_ZONE) {
			//this is a confirmed swipe
			float x = swipeDelta.x;
			float y = swipeDelta.y;

			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				//left or right
				if (x < 0)
					swipeLeft = true;
				else
					swipeRight = true;
			} else {
				//up or down
				if (y < 0)
					swipeDown = true;
				else
					swipeUp = true;
			}

			startTouch = swipeDelta = Vector2.zero;
		}
	}
}
