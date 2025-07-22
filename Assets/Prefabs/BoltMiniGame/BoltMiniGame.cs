//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEngine;

//public class BoltMiniGame : MonoBehaviour
//{
//    [SerializeField] private List<GameObject> bolts;

//    [SerializeField] private List<GameObject> layers;

//    private bool isInside = false;

//    private void OnTriggerEnter2D(Collider2D bolt)
//    {
//        isInside = true;
//    }

//    private void Start()
//    {
//        foreach (Collider2D bolt_collider in bolts)
//        {
//            OnTriggerEnter2D(bolt_collider);
//            if (isInside = true)
//            {
//                bolt_collider.enabled = false;
//            }
//        }
//    }
//}



using UnityEngine;

public class BoltController : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 startPos;
    private Collider2D currentHole;

    void Start()
    {
        startPos = transform.position; // Save starting position
    }

    void OnMouseDown()
    {
        // Calculate offset between mouse position and bolt
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, transform.position.z);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z) + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Check if bolt is on top of a valid hole
        if (currentHole != null)
        {
            // Snap bolt to hole position
            transform.position = currentHole.transform.position;
            // Optionally disable dragging
            this.enabled = false;
        }
        else
        {
            // Return bolt to start position
            transform.position = startPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoltHole"))
        {
            currentHole = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentHole)
        {
            currentHole = null;
        }
    }
}


