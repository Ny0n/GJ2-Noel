using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AIMovement : MonoBehaviour
{
    private bool _colliding;
    private bool _grounded;
    private float _angle;
    private float _currentSpeed;
    private float _forwardAxisValue;
    private float _leftAxisValue;
    private float[] _lastHitRayCastDistance;
    private Rigidbody _rigidbody;

    [SerializeField] private float _accelerationPerFrame;
    [SerializeField] private float _angleSpeed;
    [SerializeField] private float _dampening;
    [SerializeField] private float _deccelerationPerFrame;
    [SerializeField] private float _length;
    [SerializeField] private float _speed;
    [SerializeField] private int _currentTarget;
    [SerializeField] private int _strength;
    [SerializeField] private LayerMask _raycastIgnore;
    [SerializeField] private Transform[] _waypoints;

    [SerializeField] protected Transform _centerOfMass;
    [SerializeField] protected Transform[] _hoverPoints;

    // Start is called before the first frame update
    void Start()
    {
        _currentTarget = 0;
        _currentSpeed = 0;
        _lastHitRayCastDistance = new float[_hoverPoints.Length];

        for (int i = 0; i < _lastHitRayCastDistance.Length; i++)
        {
            _lastHitRayCastDistance[i] = _length;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = _waypoints[_currentTarget].position + new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
        Vector3 heading = targetPosition - transform.position;
        Vector3 headingNext;
        if (_waypoints.Length == _currentTarget + 1)
            headingNext = _waypoints[0].position - transform.position;
        else
            headingNext = _waypoints[_currentTarget + 1].position - transform.position;

        Vector3 perp = Vector3.Cross(transform.forward, heading);
        float dir = Vector3.Dot(perp, transform.up);

        float angleDiff = Mathf.Abs(Vector3.SignedAngle(transform.forward, heading, transform.forward));
        float secondAngleDiff = Mathf.Abs(Vector3.SignedAngle(transform.forward, headingNext, transform.forward));

        if (_currentSpeed < 0.25)
        {
            Debug.Log("Pas assez vite !");
        }

        Debug.DrawRay(transform.position, transform.forward);

        if (angleDiff > -_angleSpeed*1.5 && angleDiff < _angleSpeed*1.5 && secondAngleDiff > -_angleSpeed*3 && secondAngleDiff < _angleSpeed*3)
            ForwardAxis(1);
        else
            ForwardAxis(-1f);

        if (dir > 0f)
            LeftAxis(1f);
        else if (dir < 0f)
            LeftAxis(-1f);
        else
            LeftAxis(0f);

        if (_forwardAxisValue != 0 && !_colliding && _currentSpeed < _speed && _currentSpeed > -_speed)
        {
            _currentSpeed += Time.deltaTime * _accelerationPerFrame * _forwardAxisValue;

        }
        else
            _currentSpeed += (Time.deltaTime * _deccelerationPerFrame * (_currentSpeed > 0 ? -1 : 1));

        if (Vector3.Distance(transform.position, targetPosition) < 10)
        {
            Debug.Log("Changement de waypoint");
            if (_waypoints.Length == _currentTarget + 1)
            {
                Debug.Log("Tour termin�");
                _currentTarget = 0;
            }
            else
                _currentTarget++;
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < _waypoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_waypoints[i].position, _waypoints[(int)Mathf.Repeat(i + 1, _waypoints.Length)].position);
        }
    }

    void LateUpdate()
    {
        _angle = 0;

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
        if (!_grounded)
            return;

        FaceForwardWithUPDependingBarycentricCoordinate();


        _angle = _angleSpeed * _leftAxisValue;

        if (!_colliding)
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
        Debug.Log("collide !");
        if (collision.gameObject.tag == "ground")
            return;

        Vector3 speed = Vector3.Project(_rigidbody.velocity.normalized, Vector3.forward);
        float magnitude = _rigidbody.velocity.magnitude;

        ContactPoint contact = collision.contacts[0];
        Vector3 collisionVector = Vector3.Project(transform.position - contact.point, Vector3.forward);
        //Debug.DrawRay(transform.position, collisionVector * 1000, Color.green, 25f);
        speed = (collisionVector + speed).normalized;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.velocity = (speed * magnitude * 0.1f);
        _currentSpeed = 0;
        _colliding = true;

        CancelInvoke();
        Invoke("SetCollidingToFalse", 1f);
    }

    private void ForwardAxis(float value)
    {
        _forwardAxisValue = value;
    }

    private void LeftAxis(float value)
    {
        _leftAxisValue = value;
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

            //Debug.DrawRay(transform.position, interpolatedNormal * 1000, Color.white, Mathf.Infinity);
            //Debug.DrawRay(transform.position, transform.up * 1000, Color.yellow, Mathf.Infinity);
        }
        else
        {
            Debug.Log("Ho no");
        }

    }

    private void SetCollidingToFalse()
    {
        _colliding = false;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
