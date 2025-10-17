using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class TorchSfx : MonoBehaviour
{
    public string torchSfx;
    private void Start()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlaySfx(torchSfx);
    }
}