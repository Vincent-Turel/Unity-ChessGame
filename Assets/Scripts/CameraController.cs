using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public float verticalScrollArea = 10f;
	public float horizontalScrollArea = 10f;
	public float verticalScrollSpeed = 10f;
	public float horizontalScrollSpeed = 10f;
	
	public void EnableControls(bool _enable) {
		
		if(_enable) {
			ZoomEnabled = true;
			MoveEnabled = true;
			CombinedMovement = true;
		} else {
			ZoomEnabled = false;
			MoveEnabled = false;
			CombinedMovement = false;
		}
	}
	
	public bool ZoomEnabled = true;
	public bool MoveEnabled = true;
	public bool CombinedMovement = true;
	
	private Vector2 _mousePos;
	private Vector3 _moveVector;
	private float _xMove;
	private float _yMove;
	private float _zMove;
	
	void Update () {
		_mousePos = Input.mousePosition;
		
		//Move camera if mouse is at the edge of the screen
		if (MoveEnabled) {
			
			//Move camera if mouse is at the edge of the screen
			if (_mousePos.x < horizontalScrollArea)
			{
				_xMove = -1;
			}
			else if (_mousePos.x >= Screen.width - horizontalScrollArea) {
				_xMove = 1;
			}
			else {
				_xMove = 0;
			}
			
			if (_mousePos.y < verticalScrollArea) {
				_zMove = -1;
			}
			else if (_mousePos.y >= Screen.height - verticalScrollArea) {
				_zMove = 1;
			}
			else {
				_zMove = 0;
			}
			
			//Move camera if wasd or arrow keys are pressed
			float xAxisValue = Input.GetAxis("Horizontal");
			float zAxisValue = Input.GetAxis("Vertical");
			
			if (xAxisValue != 0) {
				if (CombinedMovement) {
					_xMove += xAxisValue;
				}
				else {
					_xMove = xAxisValue;
				}
			}
			
			if (zAxisValue != 0) {
				if (CombinedMovement) {
					_zMove += zAxisValue;
				}
				else {
					_zMove = zAxisValue;
				}
			}
			
		}
		else {
			_xMove = 0;
			_yMove = 0;
		}
		
		// Zoom Camera in or out
		if(ZoomEnabled) {
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				_yMove = 1;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				_yMove = -1;
			}
			else {
				_yMove = 0;
			}
		}
		else {
			_zMove = 0;
		}
		
		//move the object
		MoveMe(_xMove, _yMove, _zMove);
	}
	
	private void MoveMe(float x, float y, float z) {
		_moveVector = (new Vector3(x * horizontalScrollSpeed,
		                           y * verticalScrollSpeed, z * horizontalScrollSpeed) * Time.deltaTime);
		transform.Translate(_moveVector, Space.World);
	}
}