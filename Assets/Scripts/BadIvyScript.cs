using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class BadIvyScript : MonoBehaviour
{
    public LayerMask l;
    FloorLevelScript floorLevel;
    Animator animator;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorLevel = FindAnyObjectByType<FloorLevelScript>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (float) Math.Pow((player.transform.position.x - transform.position.x),2) + (float) Math.Pow((player.transform.position.y - transform.position.y), 2);
        if(Math.Sqrt(distance) <= 1)
        {
            floorLevel.badIvy(transform.position);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //floorLevel.hitBadIvy();
    }
}
