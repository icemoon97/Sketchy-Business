using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform levelSelectCameraPos;
    public Transform galleryCameraPos;
    public Transform mainCameraPos;

    public float moveSpeed = 0.125f;

    public void LevelSelectTransition()
    {
        StopAllCoroutines();
        StartCoroutine(Move(levelSelectCameraPos));
    }

    public void GalleryTransition()
    {
        StopAllCoroutines();
        StartCoroutine(Move(galleryCameraPos));
    }

    public void MainMenuTransition()
    {
        StopAllCoroutines();
        StartCoroutine(Move(mainCameraPos));
    }

    //IEnumerator makes it a coroutine
    private IEnumerator Move(Transform target)
    {
        while (Vector3.Distance(target.position, transform.position) > 0.01f)
        {
            //Moves the camera smoothly
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target.position, moveSpeed);
            transform.position = smoothedPosition;

            //Rotates the camera smoothly
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, target.rotation, moveSpeed);
            transform.rotation = smoothedRotation;

            yield return new WaitForSeconds(0.02f);
        }
    }
}
