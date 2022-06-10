using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    [SerializeField]
    private GameObject player;

    private float landingYpos;
    void Start()
    {
        landingYpos = 0;
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, landingYpos, 0);
    }
    public void PlayLanding()
    {
        landingYpos = player.transform.position.y - 0.3f;
        ps.Clear();
        ps.Play();
    }
}
