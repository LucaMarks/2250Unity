using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StonePlatform : MonoBehaviour
{
    public GameObject thisObject;
    public int moveFactor = 1;
    private Vector3 startPos;
    // private Vector3 prevPos;
    private bool canMove = false;
    [SerializeField] private float moveDuration = 0.35f;

    private Coroutine moveRoutine;

    public void movePlatform(int dir)
    {
        switch (dir)
        {
            case 0: moveRight(); break;
            case 1: moveForward(); break;
            case 2: moveLeft(); break;
            case 3: moveDown(); break;
        }
    }

    public void Awake()
    {
        if (thisObject == null)
        {
            thisObject = gameObject;
        }

        startPos = thisObject.transform.position;
    }

    public void Reset()
    {
        if (thisObject == null)
        {
            thisObject = gameObject;
        }

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        if (canMove)
        {
            thisObject.transform.position = startPos;
        }
        else
        {
            canMove = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with stone");
        StonePlatform platform = collision.gameObject.GetComponent<StonePlatform>();
        if (platform != null)
        {
            canMove = false;
            Reset();
            // thisObject.transform.position = prevPos;
        }
    }

    private void moveRight()
    {
        BeginMove(Vector3.left);
    }

    private void moveForward()
    {
        BeginMove(Vector3.back);
    }

    private void moveLeft()
    {
        BeginMove(Vector3.right);
    }

    private void moveDown()
    {
        BeginMove(Vector3.forward);
    }

    private void BeginMove(Vector3 direction)
    {
        // canMove = true;
        if (thisObject == null)
        {
            thisObject = gameObject;
        }

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        Vector3 targetPosition = thisObject.transform.position + (direction * moveFactor);
        moveRoutine = StartCoroutine(AnimateMove(targetPosition));
    }
    private IEnumerator AnimateMove(Vector3 targetPosition)
    {
        Transform platformTransform = thisObject.transform;
        Vector3 initialPosition = platformTransform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / moveDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            platformTransform.position = Vector3.Lerp(initialPosition, targetPosition, easedProgress);
            yield return null;
        }

        platformTransform.position = targetPosition;
        moveRoutine = null;
        // prevPos = thisObject.transform.position;
    }
}
