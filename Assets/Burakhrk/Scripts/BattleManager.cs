using UnityEngine;
using UnityEngine.Events;

using Unity;
public  class BattleManager :MonoBehaviour
{
   public UnityAction  OnBattle;
    public bool Trigger = false;
    public GameObject ScreenStick;

    public void OnBattleInvoke()
    {
        OnBattle?.Invoke();
        ScreenStick.SetActive(false);
        Trigger = true;
        Debug.Log("BattleTime");
    }
}
