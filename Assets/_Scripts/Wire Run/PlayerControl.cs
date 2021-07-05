using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	public float speed = 5.0f;
	private bool canMove = false;
	public bool isEndless = false;
	public Transform model;
	public Vector3 modelRotationSpeed;
	public int explosionID;

	private void Start()
	{
		EventManager.StartListening("MakeGame", SetMoveOn);
	}

	private void OnDisable()
	{
		EventManager.StopListening("MakeGame", SetMoveOn);
	}

	private void SetMoveOn(EventParameter evn)
	{
		canMove = true;
		isEndless = GameManager.Instance.isEndless;
	}

	private void Update()
    {
		if (canMove)
        {
			if (Input.touchCount > 0)
			{
				Touch touchInput = Input.GetTouch(0);

				if (touchInput.phase == TouchPhase.Stationary || touchInput.phase == TouchPhase.Moved)
				{
					Vector3 touchedPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchInput.position.x, touchInput.position.y, 50));
					if (touchedPosition.x > 12) touchedPosition.x = 12;
					else if (touchedPosition.x < -12) touchedPosition.x = -12;
					Vector3 newPos = new Vector3(touchedPosition.x, transform.position.y, transform.position.z);
					transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * speed);
					//Vector3 touchedposition = Camera.main.ScreenToWorldPoint(touchInput.position);
					//Vector3 newPos = new Vector3(touchedposition.x, transform.position.y, transform.position.z);
					//transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * speed);
				}
			}
			if (Input.GetMouseButton(0))
			{
				Vector3 touchedPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50));
				if (touchedPosition.x > 12) touchedPosition.x = 12;
				else if (touchedPosition.x < -12) touchedPosition.x = -12;
				Vector3 newPos = new Vector3(touchedPosition.x, transform.position.y, transform.position.z);
				transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * speed);
			}
		}

		model.Rotate(modelRotationSpeed * Time.deltaTime);
	}
}
