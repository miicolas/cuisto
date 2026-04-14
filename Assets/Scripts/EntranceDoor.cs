using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private DoorController doorController;

    void Start()
    {
        doorController = GetComponentInParent<DoorController>();
    }

    void Update()
    {
        // Cherche le Player dans un rayon de 1.5 unités autour de la porte
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player détecté !");
                doorController.isBeingPushed = true;
            }
        }
    }
}