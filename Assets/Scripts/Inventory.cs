using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State
{
    OnMap,
    Stored
}
public enum Owner
{
    None,
    Ruby,
    Emerald,
    Stick
}


[System.Serializable]
public class Slot
{
    public Owner owner;
    public State state;
    public bool taken;
    public int items;

    public Slot() { taken = false; items = 0; owner = Owner.None; }

}
public class Inventory : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField]
    List<Slot> slotStates;

    [SerializeField]
    List<GameObject> slots;

    [Header("States")]
    [SerializeField]
    Color empty;
    [SerializeField]
    Color occupied;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var it in slotStates)
        {
            it.owner = Owner.None;
            it.items = 0;
            it.state = State.OnMap;
        }
        for (int i = 0; i < slots.Count; i++)
        {
           slots[i].GetComponentInChildren<TextMesh>().text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        RefreshSlots();
    }
    bool done = false;
    private void RefreshSlots()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int pickup = 0; pickup < GameObject.FindGameObjectsWithTag("Pickup").Length; pickup++)
        {

            for (int slot = 0; slot < slots.Count; slot++)
            {
                if (slots[slot].GetComponent<BoxCollider2D>().OverlapPoint(mouse))
                    slots[slot].GetComponentInChildren<SpriteRenderer>().color = occupied;
                // Use the default fill in
                else
                    slots[slot].GetComponentInChildren<SpriteRenderer>().color = empty;

                if (slots[slot].GetComponent<BoxCollider2D>().OverlapPoint(mouse) && Input.GetMouseButtonUp(0) &&
                    slots[slot].GetComponent<BoxCollider2D>().IsTouching(GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<BoxCollider2D>()) &&
                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().state == State.OnMap)
                {

                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponentInChildren<Light>().enabled = false;
                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<SpriteRenderer>().material = GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().def;

                    // Check if this slot is already owned by a different item (Pickup cannot stack on emerald for example)
                    if (slotStates[slot].owner == GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().owner || slotStates[slot].owner == Owner.None)
                    {

                        GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().storage = Storage.Inventory;

                        slotStates[slot].owner = GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().owner;
                        GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().slotNo = slot;

                        GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().state = State.Stored;

                        GameObject.FindGameObjectsWithTag("Pickup")[pickup].gameObject.transform.position = slots[slot].transform.position;
                        slotStates[slot].items++;
                        slots[slot].GetComponentInChildren<TextMesh>().text = slotStates[slot].items.ToString();
                    }
                    // Reset the item back to when it was picked up (look in Pickup.cs)
                    else
                        GameObject.FindGameObjectsWithTag("Pickup")[pickup].transform.position = GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().lastHeld;

                }
                //if (GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<BoxCollider2D>().IsTouching(slots[slot].GetComponent<BoxCollider2D>()))
                //Debug.Log(slot.ToString() + GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<BoxCollider2D>().IsTouching(slots[slot].GetComponent<BoxCollider2D>()));

                //Debug.Log(GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<BoxCollider2D>().isTrigger);

                if (slots[slot].GetComponent<BoxCollider2D>().OverlapPoint(mouse) && Input.GetMouseButtonDown(0) &&
                   GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().state == State.Stored &&
                   GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<BoxCollider2D>().IsTouching(slots[slot].GetComponent<BoxCollider2D>()))
                {
                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().storage = Storage.None;

                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponentInChildren<Light>().enabled = true;
                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<SpriteRenderer>().material = GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().illuminated;

                    GameObject.FindGameObjectsWithTag("Pickup")[pickup].GetComponent<Pickup>().state = State.OnMap;

                    slotStates[slot].owner = Owner.None;

                    slotStates[slot].items--;

                    if (slotStates[slot].items == 0)
                        slots[slot].GetComponentInChildren<TextMesh>().text = "";
                    else
                        slots[slot].GetComponentInChildren<TextMesh>().text = slotStates[slot].items.ToString();

                }
            }
        }

    }
    public void AddItem(GameObject go)
    {
        //Debug.Log("Adding item to inventory...");
        bool add = false;

        for (int i = 0; i < slotStates.Count; i++)
        {
            if ((slotStates[i].owner == go.GetComponent<Pickup>().owner || slotStates[i].owner == Owner.None) && add == false)
            {
                go.GetComponentInChildren<Light>().enabled = false;
                go.GetComponent<SpriteRenderer>().material = go.GetComponent<Pickup>().def;

                go.GetComponent<Pickup>().storage = Storage.Inventory;
                add = true;
                go.transform.position = slots[i].transform.position;
                Debug.Log(go.transform.position.ToString());
                slotStates[i].owner = go.GetComponent<Pickup>().owner;
                go.GetComponent<Pickup>().slotNo = i;
                //Debug.Log(go.GetComponent<Pickup>().owner.ToString());

                go.GetComponent<Pickup>().state = State.Stored;
                slotStates[i].items++;
                slots[i].GetComponentInChildren<TextMesh>().text = slotStates[i].items.ToString();
                RefreshSlots();
                //go.GetComponent<Pickup>().pickedUpAndInInventory = true;
            }
        }
        if (!add)
            Debug.Log("Inventory Full!");

        //RefreshSlots();
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        //Debug.Log(slots[0].transform.position.ToString() + slots[1].transform.position.ToString());
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Pickup").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Pickup")[i].GetComponent<Pickup>().state == State.Stored && GameObject.FindGameObjectsWithTag("Pickup")[i].GetComponent<Pickup>().storage == Storage.Inventory)
            {
                GameObject.FindGameObjectsWithTag("Pickup")[i].transform.position = slots[GameObject.FindGameObjectsWithTag("Pickup")[i].GetComponent<Pickup>().slotNo].transform.position;
            }
        }
    }
}
