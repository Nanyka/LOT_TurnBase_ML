using GOAP;

public class NPCAction : GAction
{
    protected NPCBrain _mBrain;
    
    private void Start()
    {
        _mBrain = GetComponent<NPCBrain>();
    }

    public override bool PrePerform()
    {
        throw new System.NotImplementedException();
    }

    public override bool PostPerform()
    {
        throw new System.NotImplementedException();
    }
}
