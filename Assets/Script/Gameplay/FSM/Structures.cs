using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackAnimation
{
    public AnimationClip anim;
    public AnimationCurve curve;
    public float percentTimeOfStartHitbox;
    public float percentTimeOfEndHitbox;
}

[Serializable]
public class MovementAnimation
{
    public AnimationClip anim;
    public Animation curve;
    public float percentTimeOfStartIframe;
    public float percentTimeOfEndIframe;
}

[Serializable]
public class DefenseAnimation
{
    public AnimationClip anim;
}

[Serializable]
public class AttackCooldown
{
    public AttackType attackType;
    public float cooldown;
}

[Serializable]
public class Sound
{
    public SoundId id;
    public AudioClip audio;
}
