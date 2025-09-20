using System;
using UnityEngine;

namespace Suburb.Inputs
{
    public interface IPointerSession : ISession
    {
        public bool CheckIncludeInBounds(Vector2 point);
    }
}