using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    bool isFiring;
    bool semiAuto;
    // Start is called before the first frame update
    Vector3 hitPos;
    Vector3 targetPos;

    bool isLaserActive = false;
    float laserTime = 0.5f;
    float laserTimer = 0.0f;

    bool audioTrigger;

    // GameObject myCamera;
    void Start()
    {
        // myCamera = GameObject.FindGameObjectWithTag("MainCamera");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!semiAuto)
            {
                isFiring = true;
                semiAuto = true;
                isLaserActive = true;

                // Vector3 mousePos = Input.mousePosition;
                // mousePos.z = Camera.main.nearClipPlane;
                // targetPos = Camera.main.ScreenToWorldPoint(mousePos);
                Debug.Log($"mouse- {Input.mousePosition}");
                Debug.Log($"target- {targetPos}");
            }
            // var lr = GetComponentInChildren<LineRenderer>();
            // lr.SetPosition(0, Vector3.right);
            // var localPos = transform.InverseTransformPoint(hitPos);
            // lr.SetPosition(1, localPos);
        }
        else
        {
            semiAuto = false;
        }

        if (isLaserActive)
        {
            var audioSource = GetComponentInChildren<AudioSource>();
            if (!audioTrigger)
            {
                // audioSource.Play();
                audioTrigger = true;
                audioSource.PlayOneShot(audioSource.clip, 1.0f);
            }

            laserTimer += Time.deltaTime;

            var lr = GetComponentInChildren<LineRenderer>();
            lr.enabled = true;
            if (laserTimer >= laserTime)
            {
                isLaserActive = false;
                laserTimer = 0f;
                lr.enabled = false;
                isFiring = false;
                audioTrigger = false;
            }

            // Determine World Coordinates of Mouse position
            float distance;
            Plane plane = new Plane(Vector3.up, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Vector3 worldPosition;
            if (plane.Raycast(ray, out distance))
            {
                targetPos = ray.GetPoint(distance);
            }

            lr.SetPosition(0, Vector3.right);
            var localPos = transform.InverseTransformPoint(hitPos);
            lr.SetPosition(1, localPos);
            // hitChunk.ResolveHit(hit, transform);
        }


    }

    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {
        // isFiring = true;
        if (isLaserActive)
        {
            // isFiring = false;
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            var heading = targetPos - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            // Does the ray intersect any objects excluding the player layer
            // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))\
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log($"Hit {hit.point.x}, {hit.point.y}, {hit.point.z}");

                var hitChunk = hit.transform.GetComponent<TerrainChunk>();
                hitPos = hit.point;
                // var lr = GetComponentInChildren<LineRenderer>();
                // lr.SetPosition(0, Vector3.right);
                // var localPos = transform.InverseTransformPoint(hitPos);
                // lr.SetPosition(1, localPos);
                hitChunk.ResolveHit(hit, transform);
            }
            else
            {
                hitPos = targetPos;
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }
}
