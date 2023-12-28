using System;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public enum ButtonRequire
{
    NONE,
    SKIP,
    STARTBATTLE
}

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string characterName;
    [TextArea] public string dialogueLine;
    public int dialogueSize;
    public ButtonRequire buttonRequire;

    private bool clipPlayed = false;
    private bool pauseScheduled = false;
    private PlayableDirector director;
    private DialogUI _dialogUI;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
        // Debug.Log($"On Create timeline: {director.playableGraph.GetRootPlayable(0)}");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (pauseScheduled)
        {
            pauseScheduled = false;
            TimelineManager.Instance.PauseTimeline(director, buttonRequire);
        }
        else
        {
            if (_dialogUI != null) _dialogUI.ToggleDialogBox(false);
        }

        clipPlayed = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _dialogUI = playerData as DialogUI;

        if (!clipPlayed && info.weight > 0f)
        {
            if (_dialogUI != null)
                _dialogUI.ShowDialogLine(dialogueLine, dialogueSize, buttonRequire == ButtonRequire.SKIP);

            if (Application.isPlaying)
                if (buttonRequire != ButtonRequire.NONE)
                    pauseScheduled = true;

            clipPlayed = true;
        }
    }
}