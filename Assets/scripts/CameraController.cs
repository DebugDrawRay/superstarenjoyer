using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Revised")]
    public Tween currentTween;

    public float duration;
    public float strength;
    public int vibrato;
    public float randomness;

    public void DoScreenShake()
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness).OnComplete(() => transform.position = new Vector3(0,0, -10));
    }
}
