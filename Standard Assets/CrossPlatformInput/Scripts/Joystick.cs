using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
		}

		public Transform backImage;
		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input

		Vector3 m_StartPos, m_newStartPos, m_StartPosBackImg;
		bool m_UseX; // Toggle for using the x axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input

		void OnEnable()
		{
			CreateVirtualAxes();
		}

        void Start()
        {
            m_StartPos = transform.position;
			m_StartPosBackImg = backImage.position;
        }

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_newStartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.OnlyHorizontal);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
		}


		public void OnDrag(PointerEventData data)
		{
			Vector3 newPos = Vector3.zero;

			float angle = Vector2.Angle(data.position-new Vector2(m_newStartPos.x, m_newStartPos.y), Vector2.right);
			float mrngx = Mathf.Abs(MovementRange*Mathf.Cos(angle*Mathf.Deg2Rad));

			if (m_UseX)
			{
				float delta = data.position.x - m_newStartPos.x;
				delta = Mathf.Clamp(delta, -mrngx, mrngx);
				newPos.x = delta;
			}

			transform.position = new Vector3(m_newStartPos.x + newPos.x, m_newStartPos.y + newPos.y, m_newStartPos.z + newPos.z);
			UpdateVirtualAxes(transform.position);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			backImage.position = m_StartPosBackImg;
			UpdateVirtualAxes(m_newStartPos);
		}


		public void OnPointerDown(PointerEventData data) {
			float x=data.position.x; float y=data.position.y;
			transform.position = new Vector3(x,y); m_newStartPos=transform.position;
			backImage.position = m_newStartPos;
		}

		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
		}
	}
}