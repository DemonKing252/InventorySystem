using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed;

    Vector3 inventoryPos;
    Vector3 craftingPos;

    // Start is called before the first frame update
    void Start()
    {
        inventoryPos = FindObjectOfType<Inventory>().transform.position;
        craftingPos = FindObjectOfType<Crafting>().transform.position;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        //Debug.Log(Camera.main.transform.position);

        //transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
        transform.rotation = Quaternion.EulerAngles(0.0f, 0.0f, 0.0f);

        if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
        {
            if (Input.GetAxis("Horizontal") < 0.0f)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX = false;

            //Camera.main.transform.position += new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal"), speed * Time.deltaTime * Input.GetAxis("Vertical"), 0.0f);
            
            //transform.position += new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal"), speed * Time.deltaTime * Input.GetAxis("Vertical"), 0.0f);
            GetComponent<Animator>().enabled = true;

            transform.position += (new Vector3(speed * Time.deltaTime * Input.GetAxis("Horizontal"), speed * Time.deltaTime * Input.GetAxis("Vertical"), 0.0f));

            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);

            FindObjectOfType<Inventory>().SetPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f) + inventoryPos);
            FindObjectOfType<Crafting>().SetPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f) + craftingPos);


            //transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
        }
        else
        {
            GetComponent<Animator>().enabled = false;
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Pickup")
    //    {
    //        Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
    //    }
    //}
}
