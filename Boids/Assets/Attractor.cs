using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    // POS is short for position
    // static variable is shared by all instances of Attractor
    // public variables can be accessed by any other class (via Attractor.POS)
    static public Vector3 POS = Vector3.zero;

    // The Inscribed header signifys that these fields are meant to be modified in the
    // Inspector as settings for the component before clicking Play.
    // Fields under the Dynamic header are meant to help you see the current state of
    // the script each frame and are often overwritten by the code every frame.
    [Header( "Inscribed" )]

    public Vector3 range = new Vector3( 40, 10, 40 );

    public Vector3 phase = new Vector3 ( 0.5f, 0.4f, 0.1f );

    // FixedUpdate is called once per physics update (i.e., 50x/second)
    private void FixedUpdate()
    {
        Vector3 tPos = transform.position;
        // Mathf is a math library
        // Time is current time
        tPos.x = Mathf.Sin( phase.x * Time.time ) * range.x;

        tPos.y = Mathf.Sin(phase.y * Time.time) * range.y;
        tPos.z = Mathf.Sin(phase.z * Time.time) * range.z;
        transform.position = tPos;
        POS = tPos;
    }
}
