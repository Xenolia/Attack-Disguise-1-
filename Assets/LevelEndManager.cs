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
        OpenSafe();
        GameManager.Instance.LevelWin();
    }
    void OpenSafe()
    {
        SkinnedMeshRenderer safeRenderer= safe.GetComponent<SkinnedMeshRenderer>();
        DOVirtual.Float(0, safeRenderer.GetBlendShapeWeight(0), 0.4f, ((acceleration) => safeRenderer.SetBlendShapeWeight(0,acceleration)));
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        LevelWin();
    }
}
