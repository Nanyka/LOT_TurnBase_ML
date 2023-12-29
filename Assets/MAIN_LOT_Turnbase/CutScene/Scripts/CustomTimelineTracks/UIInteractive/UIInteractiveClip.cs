using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class UIInteractiveClip : PlayableAsset, ITimelineClipAsset
{
    public UIInteractiveBehaviour template = new ();


    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<UIInteractiveBehaviour>.Create (graph, template);
        
        return playable;
    }
    
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }
}