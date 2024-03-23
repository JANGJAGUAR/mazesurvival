using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sfx1, sfx2, sfx3, sfx4, sfx5;

    public void CreateSound()
    {
        src.clip = sfx1;
        src.Play();
    }

    public void CrushSound()
    {
        src.clip = sfx2;
        src.Play();
    }

    public void CoinSound()
    {
        src.clip = sfx3;
        src.Play();
    }

    public void FearSound()
    {
        src.clip = sfx4;
        src.Play();
    }

    public void WinSound()
    {
        src.clip = sfx5;
        src.Play();
    }
 }
