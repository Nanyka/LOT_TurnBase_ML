using UnityEngine;
using UnityEngine.Playables;

public class CharTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(!Application.isPlaying)
        {
            return;
        }

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CharTrackBehaviour> inputPlayable = (ScriptPlayable<CharTrackBehaviour>)playable.GetInput(i);
            CharTrackBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0)
            {
                Debug.Log($"Clip {i} weight {inputWeight} is active: {input.isActive}");
            }
        }
    }
}