using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SliddingDoors : MonoBehaviour
{
    public Transform Door;
    public float duration = 1f;
    private float direction;

    IEnumerator Move()
    {
        Vector3 from = Door.transform.position;
        Vector3 to = new Vector3(from.x + direction, from.y, from.z);
        
        float elapsed = 0.0f;
        while (elapsed <= duration)
        {
            Door.transform.position = Vector3.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

        yield return new WaitForSeconds(5f);

        from = Door.transform.position;
        to = new Vector3(from.x - direction, from.y, from.z);

        elapsed = 0.0f;
        while (elapsed <= duration)
        {
            Door.transform.position = Vector3.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("LeftDoor")) direction = 10f;
        else direction = -10f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter()
    {
        StartCoroutine(Move());
    }
  
}
