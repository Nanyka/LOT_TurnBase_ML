using System;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public enum ButtonRequire
{
	NONE,
	MOVE,
	DASH,
	WEAKATTACK,
	STRONGATTACK
}

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string characterName;
    [TextArea]
    public string dialogueLine;
    public int dialogueSize;
    public ButtonRequire buttonRequire;

	private bool clipPlayed = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;
	private MainUI _mainUI;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
		// Debug.Log($"On Create timeline: {director.playableGraph.GetRootPlayable(0)}");
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if(pauseScheduled)
		{
			pauseScheduled = false;
			TimelineManager.Instance.PauseTimeline(director, buttonRequire);
		}
		else
		{
			// if (_mainUI != null)
				// _mainUI.ToggleDialoguePanel(false);
		}

		clipPlayed = false;
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		_mainUI = playerData as MainUI;
		
		if(!clipPlayed && info.weight > 0f)
		{
			// _mainUI.SetDialogue(characterName, dialogueLine, dialogueSize);

			if(Application.isPlaying)
			{
				if(buttonRequire != ButtonRequire.NONE)
				{
					pauseScheduled = true;
				}
			}

			clipPlayed = true;
		}
	}
}
