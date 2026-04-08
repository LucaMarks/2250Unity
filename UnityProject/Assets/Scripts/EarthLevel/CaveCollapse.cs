using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//when the player walks through trigger, the cave collapse sequence begins:
//screen shakes, rocks fall, a message appears, then the player must run forward
public class CaveCollapse : MonoBehaviour
{
    [Header("Collapse Settings")]
    public float shakeDuration = 1.5f;
    public float shakeMagnitude = 0.15f;

    [Header("Falling Rocks")]
    //rocks dragged into this list
    public GameObject[] fallingRocks;

    [Header("UI Message")]
    //UI intergration
    public Text collapseMessage;
    public float messageDuration = 3f;

    private bool triggered = false;
    private Camera playerCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        Player player = other.GetComponent<Player>();
        if (player == null)
            player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            triggered = true;
            playerCamera = Camera.main;
            StartCoroutine(CollapseSequence());
        }
    }

    private IEnumerator CollapseSequence()
    {
        //show the warning message
        if (collapseMessage != null)
        {
            collapseMessage.text = "The cave is collapsing! Run!";
            collapseMessage.gameObject.SetActive(true);
        }

        //shake the camera and drop the rocks at the same time
        StartCoroutine(ShakeCamera());
        DropRocks();

        yield return new WaitForSeconds(messageDuration);

        //hide the message after a few seconds
        if (collapseMessage != null)
        {
            collapseMessage.gameObject.SetActive(false);
        }
    }

    private void DropRocks()
    {
        foreach (GameObject rock in fallingRocks)
        {
            if (rock != null)
            {
                //enable gravity on the rock so it falls
                Rigidbody rb = rock.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
                else
                {
                    //no rigidbody visible if it was hidden
                    rock.SetActive(true);
                }
            }
        }
    }

    private IEnumerator ShakeCamera()
    {
        if (playerCamera == null) yield break;

        Vector3 originalPos = playerCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalPos;
    }
}
