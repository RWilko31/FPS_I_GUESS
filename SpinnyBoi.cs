using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnyBoi : MonoBehaviour
{
    [SerializeField] private float flippidyfloopidyspeedyboigooey = 230f;
    [SerializeField] private float moveHeightSpeed = 10f;
    [SerializeField] private float moveHeight = 5f;
    Vector3 startPos; bool dir;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * (flippidyfloopidyspeedyboigooey * Time.deltaTime));

        if (startPos.y >= transform.position.y) dir = true;
        else if(startPos.y + (moveHeight / 10f) <= transform.position.y) dir = false;

        if (dir) transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, Time.deltaTime / moveHeightSpeed);
        else transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime / moveHeightSpeed);
    }

    private void Awake()
    {
        startPos = transform.position;
    }
}
