using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{

    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;
    [SerializeField] GameObject top;
 
    PlayerMovement pm;

    float y;
    bool reset;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        y = transform.position.y - 0.7f;
        top.SetActive(false);
        reset = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        y = 1;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!top.activeSelf && collider.transform.tag == "Player" && !Input.GetButton("Fire2"))
        {
            if (transform.eulerAngles.y == 90)
            {
                collider.transform.position = new Vector3(transform.position.x, y, collider.transform.position.z);
                if (collider.transform.position.z > leftEdge.transform.position.z)
                    collider.transform.position = new Vector3(collider.transform.position.x, y, leftEdge.transform.position.z);

                if (collider.transform.position.z < rightEdge.transform.position.z)
                    collider.transform.position = new Vector3(collider.transform.position.x, y, rightEdge.transform.position.z);
            }
            else
            {
                collider.transform.position = new Vector3(collider.transform.position.x, y, transform.position.z);
                if (collider.transform.position.x < leftEdge.transform.position.x)
                    collider.transform.position = new Vector3(leftEdge.transform.position.x, y, collider.transform.position.z);

                if (collider.transform.position.x > rightEdge.transform.position.x)
                    collider.transform.position = new Vector3(rightEdge.transform.position.x, y, collider.transform.position.z);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                y += 0.1f;
            }

            if (collider.transform.position.y >= top.transform.position.y)
            {
                top.SetActive(true);
                collider.transform.position = new Vector3(collider.transform.position.x, top.transform.position.y + 2, collider.transform.position.z);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!reset)
        {
            reset = true;
            StartCoroutine(Reseting());
        }
    }

    IEnumerator Reseting()
    {
        yield return new WaitForSeconds(4);
        top.SetActive(false);
        reset = false;
    }
}
