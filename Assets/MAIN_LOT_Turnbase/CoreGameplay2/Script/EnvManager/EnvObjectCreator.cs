using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class EnvObjectCreator : MonoBehaviour, IEnvironmentCreator
    {
        [SerializeField] private List<Vector3> _stones;
        // TODO: replace it with data based on mailHallTier
        [SerializeField] private List<CreatureData> _enemies;
        [SerializeField] private int _maxZombieAmount;
        [SerializeField] private float _spawningBreakPeriod;

        private Vector3[] _navmeshVertices;
        private int _spawningRound;

        private void Start()
        {
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            _navmeshVertices = triangles.vertices;
        }

        public void CreateEnvObjects()
        {
            foreach (var stone in _stones)
                SavingSystemManager.Instance.OnSpawnResource("Tree1", stone);

            StartCoroutine(SpawnZombie());
        }

        private IEnumerator SpawnZombie()
        {
            yield return new WaitForSeconds(_spawningBreakPeriod);
            if (_spawningRound < _maxZombieAmount)
            {
                var randomCreatureData = _enemies[Random.Range(0, _enemies.Count)];
                randomCreatureData.Position = _navmeshVertices[Random.Range(0, _navmeshVertices.Length)];
                SavingSystemManager.Instance.OnSpawnMovableEntity(randomCreatureData);
                _spawningRound++;
                StartCoroutine(SpawnZombie());
            }
        }
    }

    public interface IEnvironmentCreator
    {
        public void CreateEnvObjects();
    }
}