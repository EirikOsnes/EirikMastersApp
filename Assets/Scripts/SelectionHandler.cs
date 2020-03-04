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
    public LaserPointer pointer;
    private GameObject selector;
    private Logger logger;

    private void Start()
    {
        logger = GetComponent<Logger>();
        assignButtonParameters();
    }

    public void OnBuildingClick(GameObject go)
    {
        //Click only if laserpointer is activated
        if (pointer.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off) return;

        GameObject newSelector = Instantiate(selectionMarker, go.transform.position + new Vector3(0, go.transform.lossyScale.y + 3, 0), Quaternion.Euler(-90, 0, 0));
        DestroySelector();
        selector = newSelector;
        Test myTest = go.GetComponentInParent<Test>();
        myTest.SetSelected(go);
    }

    public void assignButtonParameters()
    {
        List<Building> buildings = GetAllBuildings();
        GameObject[] buildingGos = new GameObject[buildings.Count];
        for (int i = 0; i < buildings.Count; i++)
        {
            buildingGos[i] = buildings[i].gameObject;
        }
        foreach (GameObject go in buildingGos)
        {
            Button[] btns = go.GetComponentsInChildren<Button>();
            foreach (Button btn in btns)
            {
                btn.onClick.AddListener(delegate { OnBuildingClick(go); });
            }
        }
    }

    public void DestroySelector()
    {
        if (selector != null) Destroy(selector);
    }

    public bool IsPointing()
    {
        return pointer.laserBeamBehavior == LaserPointer.LaserBeamBehavior.On;
    }

    public bool HasSelected()
    {
        return selector != null;
    }

    List<Building> GetAllBuildings()
    {
        List<Building> objectsInScene = new List<Building>();

        foreach (Building b in Resources.FindObjectsOfTypeAll(typeof(Building)) as Building[])
        {
            if (/*!EditorUtility.IsPersistent(t.transform.root.gameObject) &&*/ !(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(b);
        }

        return objectsInScene;
    }
}
