using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform _center;
    [SerializeField] private LayerMask _raycastIgnore;
    [SerializeField] protected Transform[] _hoverPoints;

    [SerializeField] private float _speed;
    [SerializeField] private float _accelerationPerFrame;
    [SerializeField] private float _deccelerationPerFrame;
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
    private float _currentSpeed;
    private float _forwardAxisValue;
    private float _leftAxisValue;
    // Start is called before the first frame update
    void Start()
    {
        _currentSpeed = 0;
        _lastHitRayCastDistance = new float[_hoverPoints.Length];

        for (int i = 0; i < _lastHitRayCastDistance.Length; i++)
        {
            _lastHitRayCastDistance[i] = _length;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    private void Update()
    {
        //if (IsOwner) 
        //{ 
        //    if (Input.GetKey(KeyCode.Z))
        //        ForwardAxis(1);
        //    else if(Input.GetKey(KeyCode.S))
        //        ForwardAxis(-1);
        //    else
        //        ForwardAxis(0);
        //
        //
        //    if (Input.GetKey(KeyCode.Q))
        //        LeftAxis(-1);
        //    else if (Input.GetKey(KeyCode.D))
        //        LeftAxis(1);
        //    else
        //        LeftAxis(0);
        //}

        if (_forwardAxisValue != 0 && !_colliding && _currentSpeed < _speed && _currentSpeed > -_speed)
        {
            _currentSpeed += Time.deltaTime * _accelerationPerFrame * _forwardAxisValue;

        }
        else 
            _currentSpeed = _currentSpeed +(Time.deltaTime * _deccelerationPerFrame *(_currentSpeed > 0 ? -1 : 1));


    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        Vector2 vec2 = context.ReadValue<Vector2>();
        if (vec2 != null)
        {
            if (vec2.x > 0)
                LeftAxis(1);
            else if (vec2.x < 0)
                LeftAxis(-1);
            else
                LeftAxis(0);
            if (vec2.y > 0) 
                ForwardAxis(1);
            else if (vec2.y < 0)
                ForwardAxis(-1);
            else
                ForwardAxis(0);
        }
    }

    private void ForwardAxis(float value)
    {
        ForwardAxisServerRPC(value);
    }

    private void LeftAxis(float value)
    {
        LeftAxisServerRpc(value);
    }

    [ServerRpc]
    private void ForwardAxisServerRPC(float value) 
    {
        _forwardAxisValue = value;
    }

    [ServerRpc]
    private void LeftAxisServerRpc(float value)
    {
        _leftAxisValue = value;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!IsServer)
            return;
        if (!IsOwner)
            Debug.Log(_leftAxisValue + " " + _forwardAxisValue);

        RaycastHit hit;
        _grounded = true;
        for (int i = 0; i < _hoverPoints.Length; i++)
        {
            if (Physics.Raycast(_hoverPoints[i].position, -_hoverPoints[i].up, out hit, _length, ~_raycastIgnore) && hit.distance != _length)
            {
                float forceAmount = _strength * (_length - hit.distance) / _length + (_dampening * (_lastHitRayCastDistance[i] - hit.distance));
                _rigidbody.AddForceAtPosition(transform.up * forceAmount, _hoverPoints[i].position);
                _lastHitRayCastDistance[i] = hit.distance;

            }
            else
            {
                Vector3 dowmVector = (-transform.up - Vector3.up).normalized;
                _rigidbody.AddForceAtPosition((dowmVector * 9.81f * Time.fixedDeltaTime), _hoverPoints[i].position);
                _lastHitRayCastDistance[i] = _length * 1.1f;
                _grounded = false;
            }
        }
        _angle = 0;

        
        if (!_grounded)
            return;

        FaceForwardWithUPDependingBarycentricCoordinate();


        _angle = _angleSpeed * _leftAxisValue;

        if (!_colliding )
             _rigidbody.velocity = transform.forward * _currentSpeed;



        
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
        if (collision.gameObject.tag == "ground")
            return;

        Vector3 speed = Vector3.Project(_rigidbody.velocity.normalized,Vector3.forward);
        float magnitude = _rigidbody.velocity.magnitude;

        ContactPoint contact = collision.contacts[0];
        Vector3 collisionVector = Vector3.Project(transform.position - contact.point, Vector3.forward);
        Debug.DrawRay(transform.position,collisionVector*1000,Color.green,25f);
        speed = (collisionVector + speed).normalized;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.velocity = (speed * magnitude*0.1f )  ;
        _currentSpeed = 0;
        _colliding = true;

        CancelInvoke();
        Invoke("SetCollidingToFalse", 1f);
    }
    private void SetCollidingToFalse() 
    {
        _colliding = false;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    protected virtual void FaceForwardWithUPDependingBarycentricCoordinate()
    {
        RaycastHit hit;

        if (Physics.Raycast((_hoverPoints[0].position + _hoverPoints[1].position) * 0.5f, -transform.up, out hit, Mathf.Infinity, ~_raycastIgnore))
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10);

            Debug.DrawRay(transform.position, interpolatedNormal * 1000, Color.white, Mathf.Infinity);
            Debug.DrawRay(transform.position, transform.up * 1000, Color.yellow, Mathf.Infinity);
        }
        else
        {
            Debug.Log("Ho no");
        }

    }
}
