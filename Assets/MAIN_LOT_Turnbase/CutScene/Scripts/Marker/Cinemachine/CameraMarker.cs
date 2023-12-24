using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class CameraMarker : Marker, INotification
{
    public float lenSize;
    
    public PropertyName id { get; }
}