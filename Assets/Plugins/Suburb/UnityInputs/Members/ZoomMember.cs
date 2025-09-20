using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class ZoomMember
    {
        public ReactiveCommand<(float Zoom, Vector2 Position)> OnZoom { get; } = new();
        
        public void PutZoom(float zoom, Vector2 position)
        {
            OnZoom.Execute((zoom, position));
        }
    }
}