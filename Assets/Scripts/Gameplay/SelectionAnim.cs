using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionAnim : MonoBehaviour
{

    public Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SelectCharacter()
    {
        anim.SetTrigger("isClicked");
    }
}
