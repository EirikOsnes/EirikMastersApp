﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Container for several tests ran in succession.
/// </summary>
[Serializable]
public class TestPass
{
    public List<TestData> tests = new List<TestData>();
}
