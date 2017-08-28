using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class GravityRotationManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	[Serializable]
	public struct GameObjArr {
		public GameObject[] ObjArr;	// 对象数组
		public float depth;	// 深度
	}

	[HideInInspector]
	public GravityRotationManager Instance = null;
	public bool isDrag = false;			// 是否为拖拽
	public float MaxAngle = 20f;		// 旋转最大角度

	public GameObjArr[] ObjLayers;		// 对象层级

	public float Velocity = 1.0f;

	private float _maxDistance = 0f;
	private bool _active = true;
	private int _objLen = 0;
	private float _firstDepth = 0f;

	private float _deltaX = 0f;
	private float _oldX = 0f;

	private Vector3 _gravityAttitude;
	private float _changeX;

	private void Awake() {
		Instance = this;

		//设置设备陀螺仪的开启/关闭状态，使用陀螺仪功能必须设置为 true  
		Input.gyro.enabled = true;

		if ((_objLen = ObjLayers.Length) <= 0) {
			_active = false;
		} else {
			_firstDepth = ObjLayers [0].depth;
			_maxDistance = _firstDepth * Mathf.Sin (MaxAngle / 180f * Mathf.PI);
		}
	}

	private void Update() {
		if (!_active)
			return;

		if (isDrag) {
			_changeX = _oldX - _deltaX;
			_oldX = _deltaX;
		} else {
			_gravityAttitude = Input.gyro.attitude.eulerAngles;

			ConsoleManager.Instance.print (_gravityAttitude);

			var changexAngle = _gravityAttitude.x;
			if (changexAngle > 180) {
				changexAngle = changexAngle - 360;
			} 
			if (Math.Abs(changexAngle) > MaxAngle) {
				changexAngle = changexAngle > 0 ? MaxAngle : -MaxAngle;
			}
			_changeX = _oldX - _firstDepth * Mathf.Sin (changexAngle / 180f * Mathf.PI);
			_oldX = _firstDepth * Mathf.Sin (changexAngle / 180f * Mathf.PI);
		}

		for (int i = 1; i < _objLen; i++) {
			var height = ObjLayers [i].depth - _firstDepth;
			var offset = -(_changeX / _firstDepth) * height;

			var objs = ObjLayers [i].ObjArr;
			for (int j = 0, len = objs.Length; j < len; j++) {
				var pos = objs [j].transform.position;
				pos.x += offset;
				objs [j].transform.position = pos;
			}
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (!isDrag)
			return;

		_deltaX += eventData.delta.x;
		if (Math.Abs(_deltaX) > _maxDistance) {
			_deltaX = _deltaX > 0 ? _maxDistance : -_maxDistance;
		}
	}

	public void OnPointerUp (PointerEventData eventData)
	{
	}

	public void OnPointerDown (PointerEventData eventData)
	{
	}
}