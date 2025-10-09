using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSfx : MonoBehaviour
{
    public string torchSfx;
    private void Start()
    {
        var audioManager = CoreSystem.Instance.Container.Resolve<AudioManager>();
        audioManager.PlaySfx(torchSfx);
    }
}