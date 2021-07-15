using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfx : MonoBehaviour
{
    public AudioSource hit;

    public void Playhit()
    {
        hit.Play();
    }
}
