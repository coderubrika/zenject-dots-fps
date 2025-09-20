using System;
using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace Suburb.Inputs
{
    public class RectBasedSession : CompositorsSession, IPointerSession
    {
        private readonly RectTransform bounds;
        private readonly LinkedList<RectTransform> excludedRects = new();
        
        public RectBasedSession(RectTransform bounds, RectTransform[] excludedRectsTransforms = null)
        {
            this.bounds = bounds;
            
            if (excludedRectsTransforms.IsNullOrEmpty())
                return;
            
            foreach (var rectTransform in excludedRectsTransforms)
                excludedRects.AddFirst(rectTransform);
        }
        
        public virtual bool CheckIncludeInBounds(Vector2 point)
        {
            if (bounds != null && !bounds.Contain(point))
                return false;
            
            return excludedRects.IsNullOrEmpty() || !excludedRects.Any(rect => rect.Contain(point));
        }
        
        public Vector2 TransformVector(Vector2 delta) => bounds.TransformVector(delta);
        public Vector2 TransformPoint(Vector2 position) => bounds.TransformPoint(position);

        public IDisposable AddExcludedRect(RectTransform rect)
        {
            var node = excludedRects.AddFirst(rect);
            return Disposable.Create(() => excludedRects.Remove(node));
        }
    }
}