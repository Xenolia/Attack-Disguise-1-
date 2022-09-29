using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Safe : MonoBehaviour
{
    SkinnedMeshRenderer safeRenderer;
    [SerializeField] float safeOpenDuration;
    private void Awake()
    {
        safeRenderer = GetComponent<SkinnedMeshRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            OpenSafe();
    }
    void OpenSafe()
    {
       // DOVirtual.Float(safeRenderer.GetBlendShapeWeight(0),(x)=> (safeRenderer.SetBlendShapeWeight(0,x),safeOpenDuration)) ;
        GetComponentInParent<LevelEndManager>().LevelWin();

    }
}
