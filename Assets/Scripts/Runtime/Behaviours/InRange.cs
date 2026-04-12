using System;
using PlazmaGames.Core;
using UnityEngine;

namespace ColbyO.Untitled
{
    public class InRange : MonoBehaviour
    {
        [SerializeField] private string _name = "RangeName";
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().SetInRange(_name, true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.GetMonoSystem<IGameLogicMonoSystem>().SetInRange(_name, false);
            }
        }
    }
}
