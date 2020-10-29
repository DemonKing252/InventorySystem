using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Storage
{
    None,
    Inventory,
    Crafting
}


public class Pickup : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField]
    public Material def;

    [SerializeField]
    public Material illuminated;
    
    [SerializeField]
    public Owner owner;
    public State state;

    bool held = false;

    public Storage storage;


    public Vector3 lastHeld;
    public int slotNo;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().material = illuminated;
        state = State.OnMap;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            lastHeld = mouse;
            //frame++;

            if (GetComponent<BoxCollider2D>().OverlapPoint(mouse))
            {
                held = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            held = false;

           
        }

        if (held)
        {
            transform.position = mouse;
        }

        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && state != State.Stored && !Input.GetMouseButton(0))
        {
            FindObjectOfType<Inventory>().AddItem(this.gameObject);
            
        }
    }
}
