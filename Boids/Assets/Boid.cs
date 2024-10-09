using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Neighborhood neighborhood;
    private Rigidbody rigid;

    // Use this for initialization
    void Awake()
    {
        neighborhood = GetComponent<Neighborhood>();
        rigid = GetComponent<Rigidbody>();

        // Set a random initial velocity
        vel = Random.onUnitSphere * Spawner.SETTINGS.velocity;

        LookAhead();

        Colorize();

    }


    // FixedUpdate is called once per physics update (i.e., 50x/second)
    private void FixedUpdate()
    {
        BoidSettings bSet = Spawner.SETTINGS;

        // sumVel sums all the influences acting to change the Boid's direction
        Vector3 sumVel = Vector3.zero;

        // ___ATTRACTOR___ - Move towards or away from the Attractor
        Vector3 delta = Attractor.POS - pos;
        //Check whether we're attracted or avoiding the Attractor
        if (delta.magnitude > bSet.attractPushDist)
        {
            sumVel += delta.normalized * bSet.attractPull;
        } else
        {
            sumVel -= delta.normalized * bSet.attractPush;
        }

        // In Code Listing 27.8, we'll add additional influences to sumVel here

        // ___COLLISION_AVOIDANCE___ - Avoid neighbors who are too near
        Vector3 velAvoid = Vector3.zero;
        Vector3 tooNearPos = neighborhood.avgNearPos;
        // If the response is Vector3.zero, then no need to react
        if (tooNearPos != Vector3.zero)
        {
            velAvoid = pos - tooNearPos;
            velAvoid.Normalize();
            sumVel += velAvoid * bSet.nearAvoid;
        }

        // ___VELOCITY_MATCHING___ - Try to match velocity with neighbors
        Vector3 velAlign = neighborhood.avgVel;
        // Only do more if the velAlign is not Vector3.zero
        if (velAlign != Vector3.zero)
        {
            // We're really interested in direction, so normalize the velocity
            velAlign.Normalize();
            // and then set it to the speed we chose
            sumVel += velAlign * bSet.velMatching;
        }

        // ___FLOCK_CENTERING___ - Move towards the center of local neighbors
        Vector3 velCenter = neighborhood.avgPos;
        if (velCenter != Vector3.zero)
        {
            velCenter -= transform.position;
            velCenter.Normalize();
            sumVel += velCenter * bSet.flockCentering;
        }


        // ___INTERPOLATE VELOCITY___ - Between normalized vel & sumVel
        sumVel.Normalize();
        vel = Vector3.Lerp(vel.normalized, sumVel, bSet.velocityEasing);
        // Set the magnitude of the vel to the velocity set on Spawner.SETTINGS
        vel *= bSet.velocity;

        // Look in the direction of the new velocity
        LookAhead();

    }


    // Orients the Boid to look at the direction it's flying
    void LookAhead()
    {
        transform.LookAt(pos + rigid.velocity);
    }

    // Give the Boid a random color, but make sure it's not too dark
    void Colorize()
    {
        Color randColor = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);

        // Assign the color to the Renderers of both the Fuselage and Wing
        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            r.material.color = randColor;
        }

        // Also assign the color to the TrailRenderer
        TrailRenderer trend = GetComponent<TrailRenderer>();

        trend.startColor = randColor;
        randColor.a = 0;
        trend.endColor = randColor;
        trend.endWidth = 0;
    }

    // Property used to easily get and set the position of this Boid
    public Vector3 pos
    {
        get { return transform.position; }
        private set { transform.position = value; }
    }


    // Property used to easily get and set the velocity of this Boid
    public Vector3 vel
    {
        get { return rigid.velocity; }
        private set { rigid.velocity = value; }
    }


}
