using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class Safe : MonoBehaviour
{
    [SerializeField] GameObject safeKapak;
    [SerializeField] GameObject[] moneyPrefabs; 
     [SerializeField] float safeOpenDuration;
    [SerializeField] float moneyExplosionDuration;

    [SerializeField] ParticleSystem[] explosionParticles;
    [SerializeField] Ease SafeOpenEase;


    [SerializeField] ExplosionController explosionController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            OpenSafe();
    }
    void OpenSafe()
    {
        safeKapak.transform.DORotate(new Vector3(0,130,0),safeOpenDuration,RotateMode.LocalAxisAdd).SetEase(SafeOpenEase);
        StartCoroutine(WaitForParticle());

    }
    IEnumerator WaitForParticle()
    {
        GameManager.Instance.EnableCanvas();
        FindObjectOfType<MovementInput>().acceleration = 0;
        yield return new WaitForSeconds(safeOpenDuration/2);
        foreach (var item in explosionParticles)
        {
            item.gameObject.SetActive(true);
            item.Play();
        }
        yield return new WaitForSeconds(moneyExplosionDuration);
        MoneyExplosion();

        yield return new WaitForSeconds(2);
        WinLevelTrigger();

    }
    void WinLevelTrigger()
    {
        GetComponentInParent<LevelEndManager>().LevelWin();
    }
    void MoneyExplosion()
    {
        explosionController.Explode();
    }
}
