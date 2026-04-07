using System.Collections;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int dir;
    public StonePlatform stonePlatform;
    public StonePlatform[] platforms;
    [SerializeField] private float extendAmount = 0.5f;
    [SerializeField] private float extendDuration = 0.2f;

    private Coroutine extendRoutine;

    public void Interact()
    {
        stonePlatform.movePlatform(dir);
        animateDown();
        if (dir == -1)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].Reset();
            }
        }
    }

    private void animateDown()
    {
        if (extendRoutine != null)
        {
            StopCoroutine(extendRoutine);
        }

        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.localPosition;
        Vector3 targetScale = startScale + new Vector3(0f, extendAmount, 0f);
        Vector3 targetPosition = startPosition + new Vector3(0f, extendAmount * 0.5f, 0f);

        extendRoutine = StartCoroutine(AnimateExtension(startScale, targetScale, startPosition, targetPosition));
    }

    private IEnumerator AnimateExtension(Vector3 startScale, Vector3 targetScale, Vector3 startPosition, Vector3 targetPosition)
    {
        yield return AnimateTransform(startScale, targetScale, startPosition, targetPosition);
        yield return AnimateTransform(targetScale, startScale, targetPosition, startPosition);

        transform.localScale = startScale;
        transform.localPosition = startPosition;
        extendRoutine = null;
    }

    private IEnumerator AnimateTransform(Vector3 fromScale, Vector3 toScale, Vector3 fromPosition, Vector3 toPosition)
    {
        float elapsed = 0f;

        while (elapsed < extendDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / extendDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            transform.localScale = Vector3.Lerp(fromScale, toScale, easedProgress);
            transform.localPosition = Vector3.Lerp(fromPosition, toPosition, easedProgress);
            yield return null;
        }

        transform.localScale = toScale;
        transform.localPosition = toPosition;
    }
}
