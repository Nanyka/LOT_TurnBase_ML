using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class CharControlMarker : Marker, INotification
{
    public int intParam;
    public string stringParam;
    public bool isActive;
    public bool isSpawner;
    public bool isActionOne;
    public bool isActionTwo;
    public bool isActionThree;
    
    public PropertyName id { get; }
}
