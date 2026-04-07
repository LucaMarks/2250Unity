using System.Collections;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int dir;
    public StonePlatform stonePlatform;
    public StonePlatform[] platforms;
    [SerializeField] private Transform stretchTarget;
    [SerializeField] private float extendAmount = 0.5f;
    [SerializeField] private float extendDuration = 0.2f;

    private Coroutine extendRoutine;

    private void Awake()
    {
        ResolveStretchTarget();
    }

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
        Transform target = ResolveStretchTarget();
        if (target == null)
        {
            return;
        }

        if (extendRoutine != null)
        {
            StopCoroutine(extendRoutine);
        }

        Vector3 startScale = target.localScale;
        Vector3 startPosition = target.localPosition;
        float downwardOffset = GetHalfHeightDelta(target, extendAmount);
        Vector3 targetScale = startScale + new Vector3(0f, extendAmount, 0f);
        Vector3 targetPosition = startPosition + new Vector3(0f, -downwardOffset, 0f);

        extendRoutine = StartCoroutine(AnimateExtension(target, startScale, targetScale, startPosition, targetPosition));
    }

    private IEnumerator AnimateExtension(Transform target, Vector3 startScale, Vector3 targetScale, Vector3 startPosition, Vector3 targetPosition)
    {
        yield return AnimateTransform(target, startScale, targetScale, startPosition, targetPosition);
        yield return AnimateTransform(target, targetScale, startScale, targetPosition, startPosition);

        target.localScale = startScale;
        target.localPosition = startPosition;
        extendRoutine = null;
    }

    private IEnumerator AnimateTransform(Transform target, Vector3 fromScale, Vector3 toScale, Vector3 fromPosition, Vector3 toPosition)
    {
        float elapsed = 0f;

        while (elapsed < extendDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / extendDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            target.localScale = Vector3.Lerp(fromScale, toScale, easedProgress);
            target.localPosition = Vector3.Lerp(fromPosition, toPosition, easedProgress);
            yield return null;
        }

        target.localScale = toScale;
        target.localPosition = toPosition;
    }

    private Transform ResolveStretchTarget()
    {
        if (stretchTarget != null)
        {
            return stretchTarget;
        }

        Transform cylinderChild = transform.Find("Cylinder");
        if (cylinderChild != null)
        {
            stretchTarget = cylinderChild;
            return stretchTarget;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.Contains("Cylinder") && child.name != "Curve")
            {
                stretchTarget = child;
                return stretchTarget;
            }
        }

        return null;
    }

    private float GetHalfHeightDelta(Transform target, float scaleDelta)
    {
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            return scaleDelta;
        }

        return meshFilter.sharedMesh.bounds.size.y * scaleDelta * 0.5f;
    }
}
