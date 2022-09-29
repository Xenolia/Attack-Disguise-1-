using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LevelEndManager : MonoBehaviour
{
  
    public void LevelWin()
    {
         GameManager.Instance.LevelWin();
    }
   
}
