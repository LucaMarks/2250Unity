using System.Collections.Generic;
using UnityEngine;

// Attach to a GameObject that acts as the light source for the beam puzzle
// Requires a LineRenderer component on the same GameObject
// The beam bounces off objects tagged "Mirror" and activates objects tagged "Pillar"
[RequireComponent(typeof(LineRenderer))]
public class LightBeam : MonoBehaviour
{
    public int maxBounces = 5;
    public float maxDistance = 50f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // basic beam appearance, can be changed in the inspector
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
    }

    void Update()
    {
        ShootBeam();
    }

    private void ShootBeam()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        // keep track of which pillars the beam is currently hitting
        // so we can deactivate ones the beam no longer reaches
        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, maxDistance))
            {
                points.Add(hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    // calculate bounce direction using surface normal
                    direction = Vector3.Reflect(direction, hit.normal);
                    // small offset so the new ray doesn't immediately re-hit the same mirror
                    origin = hit.point + direction * 0.01f;
                }
                else if (hit.collider.CompareTag("Pillar"))
                {
                    // beam hit a pillar - check the hit object AND its parents
                    // because the pillar may be made of child meshes
                    Pillar pillar = hit.collider.GetComponentInParent<Pillar>();
                    if (pillar != null)
                    {
                        pillar.ActivatePillar();
                    }
                    break; // beam stops at pillar
                }
                else
                {
                    // beam hit something that isn't a mirror or pillar, it stops here
                    break;
                }
            }
            else
            {
                // beam didn't hit anything, draw it to max distance
                points.Add(origin + direction * maxDistance);
                break;
            }
        }

        // update the visual line renderer with the beam path
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
