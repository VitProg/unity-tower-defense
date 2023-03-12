using UnityEngine;
using UnityEngine.AI;
using Utils;

public class Enemy : MonoBehaviour
{
    public Transform Destination;
    public float RotateSpeed = 150f;
    private NavMeshAgent agent;
    private GameObject child;
    private float direction;
    private float targetRotationAngle;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        child = transform.GetChild(0).gameObject;

        // currentRotationAngle = child.transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    private void Update()
    {
        agent.SetDestination(Destination.position);
        //Debug.Log(transform.position.ToString());

        var currentRotationAngle = GeometryUtils.NormalizeAngle(360f - child.transform.rotation.eulerAngles.z);


        if (agent.velocity.magnitude > 0)
        {
            var notNormalizedTargetAngle = GeometryUtils.AngleFromVelocity(agent.velocity);
            targetRotationAngle = GeometryUtils.NormalizeAngle(360f - notNormalizedTargetAngle + 90f);
        }
        else
        {
            targetRotationAngle = currentRotationAngle;
        }

        var delta = Mathf.Abs(currentRotationAngle - targetRotationAngle);

        if (delta > 0.01f && RotateSpeed > 0f)
        {
            var deltaA = GeometryUtils.DeltaAngle(currentRotationAngle, targetRotationAngle);
            var deltaB = GeometryUtils.DeltaAngle(targetRotationAngle, currentRotationAngle);

            direction = deltaA < deltaB ? -1 : 1;

            var newAngle = currentRotationAngle;

            var addToAngle = RotateSpeed * direction * Time.deltaTime;
            newAngle += addToAngle;
            newAngle = GeometryUtils.NormalizeAngle(newAngle);

            child.transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.back);
        }
    }
}