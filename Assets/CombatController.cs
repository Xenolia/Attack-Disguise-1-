using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    AutoAttack autoAttack;
    CombatScript combatScript;
    GameObject autoDetector;
    private void Awake()
    {
        autoAttack = GetComponent<AutoAttack>();
        combatScript = GetComponent<CombatScript>();
        autoDetector = GetComponentInChildren<DetectorPlayer>().gameObject;
    }
    public void ChangeMechanicToButton()
    {
        autoDetector.SetActive(false);
    }
    public void ChangeMechanicToAuto()
    {
        autoDetector.SetActive(true);
    }
}
