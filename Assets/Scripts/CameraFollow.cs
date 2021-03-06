﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
	public enum MoveMode { UPDATE, FIXED_UPDATE, LATE_UPDATE }
	
	public MoveMode moveMode = MoveMode.UPDATE;
	public Transform target;

    public bool InvertY = true;
    public bool InvertX = true;

	public float distance = 5f;
	public Vector2 verticalLimit = new Vector2(-15f, 15f);
	public float speed = 10f;

    public Toggle InvertXToggle;
    public Toggle InvertYToggle;

	private Vector2 m_currentRotation;

    private GameController GC;
    private GameObject Player;

    private void Awake()
    {
        GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Player = GameObject.FindGameObjectWithTag("Player");
        
        //Restore mouse invert settings
        if (PlayerPrefs.HasKey("invert_x"))
        {
            //Debug.Log("restore x");
            bool saved_x_status = PlayerPrefs.GetInt("invert_x") == 1;
            InvertXToggle.isOn = saved_x_status;
            InvertX = saved_x_status;
        }
        if (PlayerPrefs.HasKey("invert_y"))
        {
            //Debug.Log("restore y"); 
            bool saved_y_status = PlayerPrefs.GetInt("invert_y") == 1;
            InvertYToggle.isOn = !saved_y_status;
            InvertY = saved_y_status;
        }
    }

    private void Start()
	{
		m_currentRotation = Vector2.zero;
    }
	
	private void Update()
	{
        if (GC.GamePaused)
            return;

        if (moveMode == MoveMode.UPDATE)
            UpdateCamera();	
	}
	
	private void FixedUpdate()
	{
        if (GC.GamePaused)
            return;

        if (moveMode == MoveMode.FIXED_UPDATE)
			UpdateCamera();
	}
	
	private void LateUpdate()
	{
        if (GC.GamePaused)
            return;

        if (moveMode == MoveMode.LATE_UPDATE)
            UpdateCamera();
	}
	
	private void UpdateCamera()
	{
		if(target == null)
			return;

        float xDir = Input.GetAxis("Mouse X") * speed;
        float yDir = Input.GetAxis("Mouse Y") * speed;

        //Debug.Log(xDir + " " + yDir);

        if (InvertX)
		    m_currentRotation.x -= xDir;
        else
            m_currentRotation.x += xDir;

        if (InvertY)
            m_currentRotation.y -= yDir;
        else
            m_currentRotation.y += yDir;

        m_currentRotation.y = ClampAngle(m_currentRotation.y, verticalLimit.x, verticalLimit.y);
		
		Quaternion rotation = Quaternion.Euler(m_currentRotation.y, m_currentRotation.x, 0.0f);
        Vector3 position =  new Vector3(0.0f, Player.transform.localScale.y / 2, -distance) + target.position;
             
        transform.rotation = rotation;
        transform.position = position;

        //Ensure player faces the same direction that the camera is facing
        Player.transform.rotation = new Quaternion(Player.transform.rotation.x, rotation.y, Player.transform.rotation.z, rotation.w);
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
         if(angle < -360.0f)
             angle += 360.0f;
         if(angle > 360.0f)
             angle -= 360.0f;
		 
         return Mathf.Clamp(angle, min, max);
     }

    public void InvertMouseX()
    {
        InvertX = !InvertX;
        if (InvertX)
            PlayerPrefs.SetInt("invert_x", 1);
        else
            PlayerPrefs.SetInt("invert_x", 0);
    }

    public void InvertMouseY()
    {
        InvertY = !InvertY;
        if (InvertY)
            PlayerPrefs.SetInt("invert_y", 1);
        else
            PlayerPrefs.SetInt("invert_y", 0);
    }
}
