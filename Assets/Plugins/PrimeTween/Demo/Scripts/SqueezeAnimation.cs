#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

public class SqueezeAnimation : MonoBehaviour {
    [SerializeField] Transform target;
    Tween tween;

    public void PlayAnimation() {
        if (!tween.IsAlive) {
            tween = Tween.LocalScale(target, new Vector3(1.15f, 0.9f, 1.15f), 0.2f, Ease.OutSine, 2, CycleMode.Yoyo);
        }
    }
}
#endif