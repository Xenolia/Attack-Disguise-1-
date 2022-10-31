using UnityEngine;

public class CombatController : MonoBehaviour
{
 
    GameObject autoDetector;
    private void Awake()
    {
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
