using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ingame
{
    public class Grappling : MonoBehaviour
    {
        public PlayerController playerController;
        public Transform gun;
        public LineRenderer lineRenderer;

        public Vector3 grapplePoint;

        public float overshootYAxis;

        public float maxGrappingDistance = 25f;
        public float forwardThrustForce = 1.0f;

        private SpringJoint joint;

        Rigidbody rigidbody;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            grapplePoint = gun.position;

            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartGrappling();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopGrappling();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (false && joint != null)
                {
                    Vector3 directionToPoint = grapplePoint - transform.position;
                    Debug.Log(directionToPoint.normalized);
                    rigidbody.AddForce(directionToPoint.normalized * forwardThrustForce, ForceMode.Impulse);

                    float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

                    joint.maxDistance = distanceFromPoint * 0.8f;
                    joint.minDistance = distanceFromPoint * 0.25f;
                }
            }
        }

        private void ExecuteGrapple()
        {
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

            if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

            JumpToPosition(grapplePoint, highestPointOnArc);
        }

        private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
        {
            float gravity = Physics.gravity.y;
            float displacementY = endPoint.y - startPoint.y;
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
                + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            return velocityXZ + velocityY;
        }

        Vector3 velocityToSet;
        public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
        {
            velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

            Invoke(nameof(SetVelocity), 0.1f);
        }

        private void SetVelocity()
        {
            rigidbody.velocity = velocityToSet;
        }

        private void StartGrappling()
        {
            Vector3 grappleDirection = Camera.main.transform.forward;
            Vector3 pivot = gun.position;

            RaycastHit hit;
            Ray ray = new(Camera.main.transform.position, grappleDirection);

            Vector3 targetPosition = pivot + grappleDirection * maxGrappingDistance;
            if (Physics.Raycast(ray,
                out hit,
                maxGrappingDistance,
                1 << LayerMask.NameToLayer("Landscape")))
            {
                targetPosition = hit.point;
            }

            grapplePoint = targetPosition;

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distacneFromPoint = Vector3.Distance(gun.position, grapplePoint);

            joint.maxDistance = distacneFromPoint * 0.8f;
            joint.minDistance = distacneFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            Invoke(nameof(ExecuteGrapple), 0.25f);
        }

        private void StopGrappling()
        {
            Destroy(joint);
        }

        public bool IsGrappling()
        {
            return joint != null;
        }

        public Vector3 GetGrapplePoint()
        {
            return grapplePoint;
        }
    }
}
