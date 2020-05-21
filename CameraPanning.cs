using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the Player object and provides the ability to pan the camera 
public class CameraPanning : MonoBehaviour
{

    public float panSpeed = 5f; //How fast the player moves
    public float panDetect = 15f; //The space from the edge of the screen that the mouse will be detected
    private float altitude; //The current player altitude

    private float moveX;
    private float moveY;
    private float moveZ;
    private float xPos;
    private float yPos;
    private Quaternion originalRotation;

    // Use this for initialization
    private void Start()
    {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {

        moveX = Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime; //Left and right movement
        moveY = Input.GetAxisRaw("Mouse ScrollWheel"); //Vertical movement
        moveZ = Input.GetAxis("Vertical") * panSpeed * Time.deltaTime; //Forward and backward movement
        xPos = Input.mousePosition.x; //The x position of the mouse
        yPos = Input.mousePosition.y; //The y position of the mouse

        if (!IngameMenu.isPaused)
        {
            if (transform.position.y >= -10 && transform.position.y <= 800) //Is the y value of the player transform position between -10 and 800
            {
                moveY *= 10; //Multiplies the moveY value by 10
                transform.Translate(new Vector3(0, moveY, 0)); //Moves the player up or down based on the moveY value
            }

            if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect) //Is the player holding down the A key or is the mouse within 15 pixels of the left edge of the screen?
            {
                moveX -= panSpeed; //Subtract the panSpeed from the moveX value
            }
            else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect) //Is the player holding down the D key or is the mouse within 15 pixels of the right edge of the screen?
            {
                moveX += panSpeed; //Add the panSpeed to the moveX value
            }

            if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect) //Is the player holding down the W key or is the mouse within 15 pixels of the top edge of the screen?
            {
                moveZ += panSpeed; //Add the panSpeed to the moveZ value
            }
            else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect) //Is the player holding down the S key or is the mouse within 15 pixels of the bottom edge of the screen?
            {
                moveZ -= panSpeed; //Subtract the panSpeed from the moveZ value
            }

            transform.Translate(new Vector3(moveX, Input.GetAxis("Mouse ScrollWheel") * -panSpeed * 0.05f, moveZ)); //Moves the player using the above values

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, 100, 900), Mathf.Clamp(transform.position.y, 60 + altitude, 75 + altitude), Mathf.Clamp(transform.position.z, 0, 850)); //Clamps the movement

            //--------------------------------
            if (Input.GetMouseButton(2))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    float halfHeight = Screen.height * 0.5f;
                    float mouseYPos = Input.mousePosition.y;
                    float differenceY = mouseYPos - halfHeight;
                    float factorY = differenceY / halfHeight;

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-45 * factorY, transform.rotation.x, 0), Time.deltaTime);
                }
                else
                {
                    float halfWidth = Screen.width * 0.5f;
                    float mouseXPos = Input.mousePosition.x;
                    float differenceX = mouseXPos - halfWidth;
                    float factorX = differenceX / halfWidth;

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90 * factorX, 0), Time.deltaTime);
                }
            }
            if(Input.GetKeyDown(KeyCode.Home))
            {
                transform.rotation = originalRotation;
            }
        }
    }
}