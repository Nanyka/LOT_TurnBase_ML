using System;
using System.Collections.Generic;
using System.Linq;
using GOAP;
using JumpeeIsland;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class GameMasterSensor : MonoBehaviour
    {
        [SerializeField] private List<GameMasterDecision> _changeDesicion;
        [SerializeField] private GameMasterDecision _spawnTree;
        [SerializeField] private GameMasterDecision _spawnGame;
        [SerializeField] private GameMasterDecision _spawnObstacle;
        [SerializeField] private GameMasterDecision _spawnChess;
        [SerializeField] private GameMasterDecision _spawnBoss;

        public void DetectEnvironment(WorldStates beliefs)
        {
            beliefs.ClearStates();

            AskForTree(beliefs);
            AskForHuntingGame(beliefs);
            AskForObstacle(beliefs);
            AskForChess(beliefs);
            AskForBoss(beliefs);
            AskForChangeDecision(beliefs);
        }

        private void AskForTree(WorldStates beliefs)
        {
            if (_spawnTree == null)
                return;
            
            // var responseDecision = _spawnTree.GetObjectsToSpawn();
            // if (responseDecision == null || !responseDecision.Any())
            //     return;
            
            // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
            //     return;
            //
            // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
        }

        private void AskForHuntingGame(WorldStates beliefs)
        {
            // if (_spawnGame == null)
            //     return;
            //
            // var responseDecision = _spawnGame.GetObjectsToSpawn();
            // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
            //     return;
            // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
        }
        
        private void AskForObstacle(WorldStates beliefs)
        {
            // if (_spawnObstacle == null)
            //     return;
            //
            // var responseDecision = _spawnObstacle.GetObjectsToSpawn();
            // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
            //     return;
            // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
        }
        
        private void AskForChess(WorldStates beliefs)
        {
            // if (_spawnChess == null)
            //     return;
            //
            // var responseDecision = _spawnChess.GetObjectsToSpawn();
            // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
            //     return;
            // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
        }
        
        private void AskForBoss(WorldStates beliefs)
        {
            // if (_spawnBoss == null)
            //     return;
            //
            // var responseDecision = _spawnBoss.GetObjectsToSpawn();
            // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
            //     return;
            // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
        }

        private void AskForChangeDecision(WorldStates beliefs)
        {
            // if (_changeDesicion == null)
            //     return;
            //
            // foreach (var decision in _changeDesicion)
            // {
            //     var responseDecision = decision.GetObjectsToSpawn();
                // if (String.IsNullOrEmpty(responseDecision.ElementAt(0)))
                //     return;
                // beliefs.ModifyState(responseDecision.ElementAt(0), 0);
            // }
        }
    }
}