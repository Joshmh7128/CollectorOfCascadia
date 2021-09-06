using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleScript : MonoBehaviour
{
    [SerializeField] GameObject[] MyObjects;
    //[SerializeField] GameObject Player;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            foreach (GameObject localObject in MyObjects)
            {
                localObject.SetActive(false);
            }
            Destroy(gameObject);
        }
    }
}
