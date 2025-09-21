using UnityEngine;
using Zenject;

namespace TestRPG
{
    public class ModalRoot : IInitializable
    {
        public Transform Root { get; private set; }
        
        public void Initialize()
        {
            Root = new GameObject("Modal Root").transform;
            Object.DontDestroyOnLoad(Root.gameObject);            
        }
    }
}