using UnityEngine;

public class TrigerCloset : MonoBehaviour
{

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPSController player = other.GetComponent<FPSController>();
            if (player != null)
            {

                player.isHidingCloset = true;
            }
        }


    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPSController player = other.GetComponent<FPSController>();
            if (player != null)
            {
                player.isHidingCloset = false;
            }
        }
    }
}

