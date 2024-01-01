using System;
using System.Linq;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class UIInteractiveBehaviour : PlayableBehaviour
{
    public bool isInteractable;

    private bool clipPlayed;
    private Selectable _selectable;

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (clipPlayed)
        {
            _selectable.interactable = !isInteractable;
            clipPlayed = false;
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!clipPlayed && info.weight > 0f)
        {
            if (_selectable == null)
                _selectable = playerData as Selectable;

            if (_selectable != null) _selectable.interactable = isInteractable;
            clipPlayed = true;
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (_selectable != null)
            _selectable.interactable = isInteractable;
    }
}