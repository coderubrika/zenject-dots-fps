using UniRx;

namespace Suburb.Inputs
{
    public class RotateMember
    {
        public ReactiveCommand<float> OnRotate { get; } = new();
        
        public void PutRotate(float angle) => OnRotate.Execute(angle);
    }
}