using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to group tests for sorting purposes.
/// </summary>
public class TestSet
{
    public List<GameObject> narrowTests = new List<GameObject>();
    public List<GameObject> fullTests = new List<GameObject>();
    public TestCreator.TestType testType;
    public int setNumber;
}
