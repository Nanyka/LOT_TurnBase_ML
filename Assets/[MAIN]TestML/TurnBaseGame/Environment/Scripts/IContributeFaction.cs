using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContributeFaction
{
    public void ContributeFaction(EnvironmentController environment, FactionType factionType, GameObject targetObject);
}
