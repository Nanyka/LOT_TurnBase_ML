using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0.5748935f,0f)]
[TrackClipType(typeof(CharTrackClip))]
[TrackBindingType(typeof(GameObject))]
public class CharTrack : PlayableTrack
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<CharTrackMixer>.Create(graph, inputCount);

        return scriptPlayable;
    }
}