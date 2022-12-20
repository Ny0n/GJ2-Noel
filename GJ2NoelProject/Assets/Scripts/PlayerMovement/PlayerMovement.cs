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

    [SerializeField] protected Transform _centerOfMass;
    private bool _grounded;
    private float _angle;
    [SerializeField] private float _angleSpeed;
    private bool _colliding;
    // Start is called before the first frame update
    void Start()
    {
        _lastHitRayCastDistance = new float[_hoverPoints.Length];

        for (int i = 0; i < _lastHitRayCastDistance.Length; i++)
        {
            _lastHitRayCastDistance[i] = _length;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        _angle = 0;

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
        if (!_grounded)
            return;

        FaceForwardWithUPDependingBarycentricCoordinate();

        if (Input.GetKey(KeyCode.Z)&& !_colliding)
        {
            _rigidbody.velocity = transform.forward * _speed;
            Debug.Log(_colliding);
        }

        if (Input.GetKey(KeyCode.Q))
            _angle -= _angleSpeed;
        if (Input.GetKey(KeyCode.D))
            _angle += _angleSpeed;


        
        Vector3 bodyRot = _angle * transform.up * Time.fixedDeltaTime;
        _rigidbody.transform.eulerAngles += bodyRot;
        if (_angle == 0)
        {
            Vector3 angularVelocity = _rigidbody.angularVelocity;
            angularVelocity.y = 0;
            _rigidbody.angularVelocity = angularVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 speed = _rigidbody.velocity.normalized;
        float magnitude = _rigidbody.velocity.magnitude;

        ContactPoint contact = collision.contacts[0];
        Vector3 collisionVector = Vector3.Project(transform.position - contact.point,transform.right);
        Debug.DrawRay(transform.position,collisionVector*1000,Color.green,25f);
        speed = (collisionVector*5 + speed).normalized;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.velocity = (speed * magnitude*0.5f )  ;
        _colliding = true;

        CancelInvoke();
        Invoke("SetCollidingToFalse", 1f);
    }
    private void SetCollidingToFalse() 
    {
        _colliding = false;
    }

    protected virtual void FaceForwardWithUPDependingBarycentricCoordinate()
    {
        RaycastHit hit;

        if (Physics.Raycast(_center.position, -transform.up, out hit, Mathf.Infinity, ~_raycastIgnore))
        {
            Debug.Log(hit.collider.name);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10);

            //Debug.DrawRay(transform.position, interpolatedNormal * 1000, Color.white, Mathf.Infinity);
            //Debug.DrawRay(transform.position, transform.up * 1000, Color.yellow, Mathf.Infinity);
        }
    }
}
