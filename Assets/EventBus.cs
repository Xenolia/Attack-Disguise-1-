using UnityEngine;
using UnityEngine.Events;

using Unity;
public  class EventBus :MonoBehaviour
{
   public UnityAction  OnBattle;
    public bool Trigger = false;

    private void Update()
    {
        if(Trigger)
        {
            OnBattle?.Invoke();
            Trigger = false;
        }
    }
}
