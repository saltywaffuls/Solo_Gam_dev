using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;

    public static GameLayer i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidObjectsLayer
    {
        get => solidObjectsLayer; 
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer; 
    }

    public LayerMask GrassLayer
    {
        get => grassLayer; 
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer; 
    }

    public LayerMask FovLayer
    {
        get => fovLayer; 
    }
}
