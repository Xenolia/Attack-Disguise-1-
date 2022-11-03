using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTransformSetter : MonoBehaviour
{
    [SerializeField] GameObject player;
    private Vector3 fogPos;
    private void Awake()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        fogPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    private void LateUpdate()
    {
        if(player!=null)
        fogPos = new Vector3(fogPos.x, player.transform.position.y, fogPos.z);
    }
}
