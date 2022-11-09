using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManagerWrapper 
{
    private static AdManagerWrapper _instance = new AdManagerWrapper();
    private static AdManager _adManager;
    public static AdManagerWrapper Instance => _instance;
    public static AdManager  AdManager => _adManager;
    public AdManagerWrapper()
    {
        _adManager = GameObject.FindObjectOfType<AdManager>();
    }
}
