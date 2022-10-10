using UnityEngine;
using UnityEngine.Events;

using Unity;
public  class BattleManager :MonoBehaviour
{
   public UnityAction  OnBattle;
    public UnityAction OnBattleFinished;

    public bool Trigger = false;
 
    public void OnBattleInvoke()
    {
        OnBattle?.Invoke();
         Trigger = true;
        Debug.Log("BattleTime");
    }
    public void BattleFinished()
    {
        OnBattleFinished?.Invoke();

        Debug.Log("Battle end");
      }
}
