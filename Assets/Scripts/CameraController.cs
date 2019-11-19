using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetPos;

    public float moveSpeed = 0.125f;

    private void Start()
    {
        targetPos.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public void StartMove()
    {
        targetPos.position = new Vector3(targetPos.position.x + 6, targetPos.position.y, targetPos.position.z);
        StartCoroutine("move");
    }

    //IEnumerator makes it a coroutine
    private IEnumerator move()
    {
        while (Vector3.Distance(targetPos.position, transform.position) > 0.01f)
        {
            //Moves the camera smoothly
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos.position, moveSpeed);

            //Rotates the camera smoothly
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, targetPos.rotation, moveSpeed);

            transform.position = smoothedPosition;
            transform.rotation = smoothedRotation;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
