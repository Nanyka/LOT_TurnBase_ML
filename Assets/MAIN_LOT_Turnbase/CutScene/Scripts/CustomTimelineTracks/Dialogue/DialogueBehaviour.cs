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
    CLICK,
    STARTGAME,
    TOBATTLE,
    DOUBLETAP
}

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public Sprite characterIcon;
    [TextArea] public string dialogueLine;
    public int dialogueWidth;
    public int dialogueHeight;
    public ButtonRequire buttonRequire;

    private bool clipPlayed = false;
    private bool pauseScheduled = false;
    private PlayableDirector director;
    private DialogUI _dialogUI;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (pauseScheduled)
        {
            pauseScheduled = false;
            TimelineManager.Instance.OnPauseTimeline.Invoke(director, buttonRequire);
        }
        else
        {
            if (_dialogUI != null) _dialogUI.ToggleDialogBox(false);
        }

        clipPlayed = false;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!clipPlayed && info.weight > 0f)
        {
            if (_dialogUI == null)
                _dialogUI = playerData as DialogUI;

            if (_dialogUI == null)
                return;

            if (characterIcon != null)
                _dialogUI.SetCharacterIcon(characterIcon);

            if (_dialogUI != null)
                _dialogUI.ShowDialogLine(dialogueLine, dialogueWidth, dialogueHeight,
                    buttonRequire == ButtonRequire.SKIP);

            if (Application.isPlaying)
                if (buttonRequire != ButtonRequire.NONE)
                    pauseScheduled = true;

            clipPlayed = true;
        }
    }
}