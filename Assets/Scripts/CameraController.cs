using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetPos;

    public float moveSpeed = 0.125f;

    private void Start()
    {
        
    }

    public void StartMove()
    {
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

            yield return new WaitForSeconds(0.02f);
        }
    }
}
