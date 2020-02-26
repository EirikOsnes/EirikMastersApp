using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{

    public Text text;
    public GameObject selectionMarker;
    private GameObject selector;
    private Logger logger;

    private void Start()
    {
        logger = GetComponent<Logger>();
        assignButtonParameters();
    }

    public void OnBuildingClick(GameObject go)
    {
            GameObject newSelector = Instantiate(selectionMarker, go.transform.position + new Vector3(0, go.transform.lossyScale.y + 3, 0), Quaternion.Euler(-90, 0, 0));
            if (selector != null) Destroy(selector);
            selector = newSelector;
            Test myTest = go.GetComponentInParent<Test>();
            myTest.SetSelected(go);
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
