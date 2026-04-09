using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The beam bounces off objects tagged "Mirror" and activates objects tagged "Pillar"
// added on/off switch function to make puzzles a little harder
[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    public int maxBounces = 5;
    public float maxDistance = 50f;

    [Header("Timed Beam Settings")]
    public float beamDuration = 4f;//how long the beam stays on
    public bool beamActive = false;//starts off (on for level 1)

    private LineRenderer lineRenderer;
    private Coroutine beamCoroutine;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;

        //beam starts in the state ticked on the script in unity
        lineRenderer.enabled = beamActive;
    }

    void Update()
    {
        if (beamActive)
        {
            ShootBeam();
        }
    }

    //called by LightSwitch when player activates it
    public void ActivateBeam()
    {
        if (beamCoroutine != null) return;//already running, ignore

        beamCoroutine = StartCoroutine(BeamTimer());//timer
    }

    private IEnumerator BeamTimer()//the yield method
    {
        beamActive = true;
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(beamDuration);//links to the coroutine, pauses at this part of the code

        //shut off the beam
        beamActive = false;
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        beamCoroutine = null;
    }

    private void ShootBeam()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, maxDistance))
            {
                points.Add(hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector3.Reflect(direction, hit.normal);
                    origin = hit.point + direction * 0.01f;
                }
                else if (hit.collider.CompareTag("Pillar"))
                {
                    Pillar pillar = hit.collider.GetComponentInParent<Pillar>();
                    if (pillar != null)//if we DID find a pillar
                    {
                        pillar.ActivatePillar();
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
            else
            {
                points.Add(origin + direction * maxDistance);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
