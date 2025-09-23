using Suburb.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace TestRPG.Input
{
    public class InputLayout : MonoBehaviour
    {
        [SerializeField] private MovePad movePad;
        [SerializeField] private TouchPad touchPad;
        [SerializeField] private Stick stick;
        [SerializeField] private Button button;

        public MovePad MovePad => movePad;

        public TouchPad TouchPad => touchPad;

        public Stick Stick => stick;

        public Button Button => button;
    }
}