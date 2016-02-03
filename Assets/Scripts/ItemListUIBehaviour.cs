using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemListUIBehaviour : MonoBehaviour {
    List<Image> itemBoxList;
    List<Image> itemImageList;
    public InventoryBehaviour inventoryBehav;
    public ItemDatabase itemDatabase;

    public GameObject baseItemBoxPrefab;
    public GameObject baseItemImagePrefab;

    public int amountOfBoxes = 10;

    void Awake() {
        itemBoxList = new List<Image>();
        itemImageList = new List<Image>();
    }

    void Start() {
        if (inventoryBehav != null) {
            inventoryBehav.InventoryChangeEvent.AddListener(OnInventoryUpdate);
        }
        InitListBoxes();
        Populate();
    }

    void InitListBoxes() {
        //Add more boxes to accomodate more inventory items than before
        for (int i = 0; i < amountOfBoxes; i++) {
            GameObject itemBox = GameObject.Instantiate<GameObject>(baseItemBoxPrefab);
            GameObject itemImage = GameObject.Instantiate<GameObject>(baseItemImagePrefab);
            itemImage.transform.SetParent(itemBox.transform);
            itemBox.transform.SetParent(transform);
            itemBoxList.Add(itemBox.GetComponent<Image>());
            itemImageList.Add(itemImage.GetComponent<Image>());
            itemBox.transform.position = new Vector3(32 + (itemBox.GetComponent<Image>().sprite.rect.width * i), itemBox.GetComponent<Image>().sprite.rect.height, 0);
        }
    }

    public void Populate() {
        //Grab the inventory array from inventory behaviour
        InventoryEntry[] inventoryArr = inventoryBehav.GetInventory();
        //Grab all infos
        for (int i = 0; i < amountOfBoxes; i++) {
            itemImageList[i].enabled = i < inventoryArr.Length;
            if (i < inventoryArr.Length) {
                itemImageList[i].sprite = itemDatabase.FindItemSprite(inventoryArr[i].item.id);
            }
        }
    }

    void Update() {

    }

    void OnInventoryUpdate() {
        Populate();
    }
}
