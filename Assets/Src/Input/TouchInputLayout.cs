using Suburb.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TestRPG.Input
{
    public class TouchInputLayout : MonoBehaviour
    {
        [SerializeField] private Button fireButton;
        [SerializeField] private MovePad movePad;
        [SerializeField] private TouchPad touchPad;

        public void Enable()
        {
            
        }
    }
}