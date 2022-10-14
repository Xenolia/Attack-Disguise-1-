using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIButtonController : MonoBehaviour
{
    [SerializeField] List<AttackButtonController> buttons;
    [SerializeField] GameObject abcPrefab;
    [SerializeField] Canvas canvas;
      EnemyDetection enemyDetection;
   
 
    public void CreateButton(EnemyScript enemy)
    {
        enemyDetection = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<EnemyDetection>();

        var abc = Instantiate(abcPrefab, canvas.transform);
        abc.GetComponentInChildren<AttackButtonController>().Init(enemy, enemyDetection);
    }
    public void DestroyButton(EnemyScript enemy)
    {
        foreach (var item in buttons)
        {
            if (item.enemyScript == enemy)
            {
                Destroy(item.gameObject);
                return;
            }
        }
    }
    public void RegisterButton(AttackButtonController button)
    {
        if (!buttons.Contains(button))
        {
            buttons.Add(button);
        }
    }

    public void UnRegisterButton(AttackButtonController button)
    {
        if (buttons.Contains(button))
        {
            buttons.Remove(button);
        }
    }
    public void DisableAllButtons()
    {
        foreach (var item in buttons)
        {
            item.DisableButtonForAWhile();
        }
    }
    public void EnableAllButtons()
    {
        foreach (var item in buttons)
        {
            item.EnableButton();
        }
    }
}
