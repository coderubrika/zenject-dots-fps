using UnityEngine;
using UnityEngine.UI;

namespace TestRPG.UI
{
    public class SliderView : MonoBehaviour
    {
        [SerializeField] private Image fill;

        public void SetValue(float value)
        {
            fill.fillAmount = value;
        }
    }
}