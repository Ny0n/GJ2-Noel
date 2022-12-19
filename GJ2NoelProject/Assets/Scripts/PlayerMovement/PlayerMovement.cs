using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _center;
    [SerializeField] private LayerMask _raycastIgnore;
    [SerializeField] protected Transform[] _hoverPoints;

    [SerializeField] private float _speed;
    [SerializeField] private float _height;
    private Rigidbody _rigidbody;
    private float[] _lastHitRayCastDistance;
    [SerializeField] private float _length;
    [SerializeField] private int _strength;
    [SerializeField] private float _dampening;
    private bool _grounded;

    // Start is called before the first frame update
    void Start()
    {
        _lastHitRayCastDistance = new float[_hoverPoints.Length];

        for (int i = 0; i < _lastHitRayCastDistance.Length; i++)
        {
            _lastHitRayCastDistance[i] = _length;
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceForwardWithUPDependingBarycentricCoordinate();

        if (Input.GetKey(KeyCode.Z))
        {
            _rigidbody.velocity = transform.forward * _speed;
        }
        else
            _rigidbody.velocity = Vector3.zero;

        RaycastHit hit;
        for (int i = 0; i < _hoverPoints.Length; i++)
        {
            if (Physics.Raycast(_hoverPoints[i].position, -_hoverPoints[i].up, out hit, _length, ~_raycastIgnore) && hit.distance != _length)
            {
                float forceAmount = _strength * (_length - hit.distance) / _length + (_dampening * (_lastHitRayCastDistance[i] - hit.distance));
                _rigidbody.AddForceAtPosition(transform.up * forceAmount, _hoverPoints[i].position);
                _lastHitRayCastDistance[i] = hit.distance;
                _grounded = true;
            }
            else
            {
                Vector3 dowmVector = (-transform.up - Vector3.up).normalized;
                _rigidbody.AddForceAtPosition((dowmVector * 9.81f * Time.fixedDeltaTime) / 4, _hoverPoints[i].position);
                _lastHitRayCastDistance[i] = _length * 1.1f;
            }
        }
    }

    protected virtual void FaceForwardWithUPDependingBarycentricCoordinate()
    {
        RaycastHit hit;

        if (Physics.Raycast(_center.position, -transform.up, out hit, Mathf.Infinity, ~_raycastIgnore))
        {
            // Just in case, also make sure the collider also has a renderer
            // material and texture
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                return;
            }

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            // Extract local space normals of the triangle we hit
            Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

            // interpolate using the barycentric coordinate of the hitpoint
            Vector3 baryCenter = hit.barycentricCoordinate;

            // Use barycentric coordinate to interpolate normal
            Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
            // normalize the interpolated normal
            interpolatedNormal = interpolatedNormal.normalized;

            // Transform local space normals to world space
            Transform hitTransform = hit.collider.transform;
            interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);


            Quaternion lookRotation = Quaternion.LookRotation(transform.forward, interpolatedNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5);
            Debug.DrawRay(transform.position, interpolatedNormal * 1000, Color.white, Mathf.Infinity);
            Debug.DrawRay(transform.position, transform.up * 1000, Color.yellow, Mathf.Infinity);
        }
    }
}
