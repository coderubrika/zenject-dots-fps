using UnityEngine;
using UnityEngine.Events;

namespace TestRPG
{
    public class BaseScreen : MonoBehaviour
    {
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}