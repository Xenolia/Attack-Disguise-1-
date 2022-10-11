using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LevelEndManager : MonoBehaviour
{
    [SerializeField] GameObject safeGate;
    BattleManager battleManager;
    private void Awake()
    {
        battleManager = GameManager.Instance.gameObject.GetComponent<BattleManager>();

    }
    private void OnEnable()
    {
        battleManager.OnBattleFinished += SafeGateAnim;
    }
    private void OnDisable()
    {
        battleManager.OnBattleFinished -= SafeGateAnim;
    }
    public void LevelWin()
    {
         GameManager.Instance.LevelWin();
    }
   public void SafeGateAnim()
    {
        if(safeGate)
        {
            safeGate.transform.DORotate
                (new Vector3(safeGate.transform.rotation.x+120, safeGate.transform.rotation.y, safeGate.transform.rotation.z), 1, RotateMode.LocalAxisAdd);
        }
    }
}