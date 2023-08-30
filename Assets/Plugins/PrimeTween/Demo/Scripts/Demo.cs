#if PRIME_TWEEN_INSTALLED
using System.Collections;
using PrimeTween;
using UnityEngine;

public class Demo : MonoBehaviour {
    [SerializeField] bool animateAutomatically;
    [SerializeField] AnimateAllType animateAllType; enum AnimateAllType { Sequence, Async, Coroutine }
    [SerializeField] Animatable[] animatables;
    [SerializeField] Wheels wheels;
    [SerializeField, Range(0.5f, 5f)] float timeScale = 1;
    bool isAnimatingWithCoroutineOrAsync;
    Sequence animateAllSequence;

    void Awake() {
        PrimeTweenConfig.SetTweensCapacity(100);
    }

    void Update() {
        Time.timeScale = timeScale;
        if (animateAutomatically && !isAnimatingWithCoroutineOrAsync && !animateAllSequence.IsAlive) {
            wheels.OnClick();
        }
    }

    public void AnimateAll(bool toEndValue) {
        if (isAnimatingWithCoroutineOrAsync) {
            return;
        }
        switch (animateAllType) {
            case AnimateAllType.Sequence:
                AnimateAllSequence(toEndValue);
                break;
            case AnimateAllType.Async:
                AnimateAllAsync(toEndValue);
                break;
            case AnimateAllType.Coroutine:
                StartCoroutine(AnimateAllCoroutine(toEndValue));
                break;
        }
    }

    /// Tweens and sequences can be grouped with and chained to other tweens and sequences.
    /// The advantage of using this method instead of <see cref="AnimateAllAsync"/> and <see cref="AnimateAllCoroutine"/> is the ability to stop/complete/pause the combined sequence.
    /// Also, this method doesn't generate garbage related to starting a coroutine or awaiting an async method.
    void AnimateAllSequence(bool toEndValue) {
        if (animateAllSequence.IsAlive) {
            animateAllSequence.Complete(); // Completing, stopping, or pausing the root sequence affects all nested tweens and sequences
            return;
        }
        
        animateAllSequence = Sequence.Create();
        foreach (var animatable in animatables) {
            var sequence = animatable.Animate(toEndValue);
            animateAllSequence.Chain(sequence);
        }
    }

    /// Tweens and sequences can be awaited in async methods.
    async void AnimateAllAsync(bool toEndValue) {
        isAnimatingWithCoroutineOrAsync = true;
        foreach (var animatable in animatables) {
            await animatable.Animate(toEndValue);
        }
        isAnimatingWithCoroutineOrAsync = false;
    }
    
    /// Tweens and sequences can also be used in coroutines with the help of ToYieldInstruction() method.
    IEnumerator AnimateAllCoroutine(bool toEndValue) {
        isAnimatingWithCoroutineOrAsync = true;
        foreach (var animatable in animatables) {
            yield return animatable.Animate(toEndValue).ToYieldInstruction();
        }
        isAnimatingWithCoroutineOrAsync = false;
    }
}
 
public abstract class Animatable : MonoBehaviour {
    public virtual void OnClick() { }
    public abstract Sequence Animate(bool toEndValue);
}
#else
using UnityEngine;

public class Demo : MonoBehaviour {
    void Awake() {
        Debug.LogError("Please install PrimeTween via 'Assets/Plugins/PrimeTween/PrimeTweenInstaller'.");
        #if !UNITY_2019_1_OR_NEWER
        Debug.LogError("And add the 'PRIME_TWEEN_INSTALLED' define to the 'Project Settings/Player/Scripting Define Symbols' to run the Demo in Unity 2018.");
        #endif
    }
}
#endif