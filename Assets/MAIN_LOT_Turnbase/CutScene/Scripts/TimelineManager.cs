using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    private PlayableDirector _activeDirector;
    private ButtonRequire _curButtonRequire;
    private bool _isPause;

    public void PauseTimeline(PlayableDirector whichOne, ButtonRequire buttonRequire)
    {
        if (_isPause == false)
        {
            _activeDirector = whichOne;
            if (_activeDirector.playableGraph.IsValid() == false)
                return;

            Debug.Log("is pause");
            _curButtonRequire = buttonRequire;
            _activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            _isPause = true;
        }
    }

    public void ResumeTimeline(ButtonRequire buttonRequire)
    {
        if (_isPause && (buttonRequire == _curButtonRequire || _curButtonRequire == ButtonRequire.NONE))
        {
            if (_activeDirector.playableGraph.IsValid() == false)
                return;
            
            Debug.Log("is resume");
            _activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
            _isPause = false;
        }
    }
}
