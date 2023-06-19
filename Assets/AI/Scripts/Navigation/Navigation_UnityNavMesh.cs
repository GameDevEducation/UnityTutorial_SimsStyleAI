using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Navigation_UnityNavMesh : BaseNavigation
{
    NavMeshAgent LinkedAgent;

    protected override void Initialise()
    {
        LinkedAgent = GetComponent<NavMeshAgent>();
    }

    protected override bool RequestPath()
    {
        LinkedAgent.speed = MaxMoveSpeed;
        LinkedAgent.angularSpeed = RotationSpeed;
        LinkedAgent.stoppingDistance = DestinationReachedThreshold;

        LinkedAgent.SetDestination(Destination);
        
        OnBeganPathFinding();

        return true;
    }

    protected override void Tick_Default()
    {

    }

    protected override void Tick_Pathfinding()
    {
        // no pathfinding in progress?
        if (!LinkedAgent.pathPending)
        {           
            if (LinkedAgent.pathStatus == NavMeshPathStatus.PathComplete)
                OnPathFound();
            else
                OnFailedToFindPath();
        }
    }

    protected override void Tick_PathFollowing()
    {
        bool atDestination = false;
        // do we have a path and we near the destination?
        if (LinkedAgent.hasPath && LinkedAgent.remainingDistance <= LinkedAgent.stoppingDistance)
        {
            atDestination = true;
        }
        else if (LinkedAgent.hasPath == false)
        {
            Vector3 vecToDestination = Destination - transform.position;
            float heightDelta = Mathf.Abs(vecToDestination.y);
            vecToDestination.y = 0f;

            atDestination = heightDelta < LinkedAgent.height && 
                            vecToDestination.magnitude <= DestinationReachedThreshold;
        }

        if (atDestination) 
        {
            OnReachedDestination();
        }
        else
        {
            if (DEBUG_ShowHeading)
                Debug.DrawLine(transform.position + Vector3.up, LinkedAgent.steeringTarget, Color.green);
        }
    }

    protected override void Tick_Animation()
    {
        float forwardsSpeed = Vector3.Dot(LinkedAgent.velocity, transform.forward) / LinkedAgent.speed;
        float sidewaysSpeed = Vector3.Dot(LinkedAgent.velocity, transform.right) / LinkedAgent.speed;

        AnimController.SetFloat("ForwardsSpeed", forwardsSpeed);
        AnimController.SetFloat("SidewaysSpeed", sidewaysSpeed);
    }

    public override void StopMovement()
    {
        LinkedAgent.ResetPath();
    }

    public override bool FindNearestPoint(Vector3 searchPos, float range, out Vector3 foundPos)
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchPos, out hitResult, range, NavMesh.AllAreas))
        {
            foundPos = hitResult.position;
            return true;
        }

        foundPos = searchPos;

        return false;
    }

}
