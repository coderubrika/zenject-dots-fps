using System;
using UniRx;

namespace Suburb.Inputs
{
    public interface IResourceDistributor
    {
        public ReactiveCommand OnAppearResources { get; }
        public bool HaveResources { get; }
        public IDisposable Enable();
    }
}