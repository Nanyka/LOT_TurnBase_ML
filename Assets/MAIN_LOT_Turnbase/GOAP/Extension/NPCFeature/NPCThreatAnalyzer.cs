using GOAP;

public class NPCThreatAnalyzer
{
    private bool _isSpeedingUp;

    public bool CheckThreatState(WorldStates targetStates)
    {
        if (_isSpeedingUp)
            return false;
        
        // Check condition here
        _isSpeedingUp = true;
        return _isSpeedingUp;
    }

    public void RefreshedSpeedUp()
    {
        _isSpeedingUp = false;
    }
}
