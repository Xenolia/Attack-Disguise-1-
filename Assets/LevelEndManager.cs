using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class LevelEndManager : MonoBehaviour
{
    [SerializeField] GameObject safe;
    [SerializeField] float safeOpenDuration;
    SkinnedMeshRenderer safeRenderer;
    float myFloat;

    public void LevelWin()
    {
        GameManager.Instance.LevelWin();
    }
    void OpenSafe()
    {
        SkinnedMeshRenderer safeRenderer= safe.GetComponent<SkinnedMeshRenderer>();
         DOTween.To(() => myFloat, x => myFloat = x, 52, 1);

        safeRenderer.SetBlendShapeWeight(0,100);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        LevelWin();
    }
}
