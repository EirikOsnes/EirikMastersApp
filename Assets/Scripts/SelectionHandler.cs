using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{

    public Text text;
    public GameObject selectionMarker;
    private GameObject currentSelected;
    private GameObject selector;

    private void Start()
    {
        assignButtonParameters();
    }

    public void OnBuildingClick(GameObject go)
    {
        currentSelected = go;
        text.text = text.text + "\n Building Clicked: Height = " + go.transform.lossyScale.y;
        GameObject newSelector = Instantiate(selectionMarker, go.transform.position + new Vector3(0, go.transform.lossyScale.y + 3, 0), Quaternion.Euler(-90, 0, 0));
        if(selector != null) Destroy(selector);
        selector = newSelector;
    }

    public void assignButtonParameters()
    {
        Building[] buildings = FindObjectsOfType<Building>();
        GameObject[] buildingGos = new GameObject[buildings.Length];
        for (int i = 0; i < buildings.Length; i++)
        {
            buildingGos[i] = buildings[i].gameObject;
        }
        foreach (GameObject go in buildingGos)
        {
            Button[] btns = go.GetComponentsInChildren<Button>();
            foreach (Button btn in btns)
            {
                btn.onClick.AddListener(delegate { OnBuildingClick(go); });
                Debug.Log("Added Button call");
            }
        }
    }
}
