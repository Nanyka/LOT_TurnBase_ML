using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CharTrackClip : PlayableAsset, ITimelineClipAsset
{
    public CharTrackBehaviour template = new();
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CharTrackBehaviour>.Create(graph, template);
        
        return playable;
    }

    public ClipCaps clipCaps { get{return ClipCaps.None; } }
}