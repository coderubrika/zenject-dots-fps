using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MPUIKIT;
using Suburb.Utils.Serialization;
using UniRx;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Suburb.Utils
{
    public static class Extensions
    {
        public static bool IsClose(this Vector2 source, float closeDistance)
        {
            return Mathf.Abs(source.x) < closeDistance
                && Mathf.Abs(source.y) < closeDistance;
        }

        public static bool IsClose(this Vector3 source, float closeDistance)
        {
            return Mathf.Abs(source.x) < closeDistance
                && Mathf.Abs(source.y) < closeDistance
                && Mathf.Abs(source.z) < closeDistance;
        }

        public static bool IsCloseWithOther(this Vector2 source, Vector2 destination, float closeDistance)
        {
            return Mathf.Abs(source.x - destination.x) < closeDistance 
                && Mathf.Abs(source.y - destination.y) < closeDistance;
        }

        public static bool IsCloseWithOther(this Vector3 source, Vector3 destination, float closeDistance)
        {
            return Mathf.Abs(source.x - destination.x) < closeDistance
                && Mathf.Abs(source.y - destination.y) < closeDistance
                && Mathf.Abs(source.z - destination.z) < closeDistance;
        }

        public static void Log<T>(this T obj, object message, string filter="")
        {
            Debug.Log($"{filter}[{typeof(T).Name}] {message}");
        }

        public static void LogWarning<T>(this T obj, object message, string filter = "")
        {
            Debug.LogWarning($"{filter}[{typeof(T).Name}] {message}");
        }

        public static void LogError<T>(this T obj, object message, string filter = "")
        {
            Debug.LogError($"{filter}[{typeof(T).Name}] {message}");
        }

        public static Vector3Data ToVector3Data(this Vector3 obj)
        {
            return new Vector3Data { X = obj.x, Y = obj.y, Z = obj.z };
        }

        public static void SetActiveGameObjects<T>(this IEnumerable<T> objects, bool isActive)
            where T : Component
        {
            foreach (T obj in objects)
                obj.gameObject.SetActive(isActive);
        }

        public static void SetActiveGameObjects(this IEnumerable<GameObject> objects, bool isActive)
        {
            foreach (GameObject obj in objects)
                obj.SetActive(isActive);
        }

        public static void DestroyGameObjects<T>(this IEnumerable<T> objects)
            where T : Component
        {
            foreach (T obj in objects)
                Object.Destroy(obj.gameObject);

            if (objects is ICollection<T> collection)
                collection.Clear();
        }

        public static void DestroyGameObjects(this IEnumerable<GameObject> objects)
        {
            foreach (GameObject obj in objects)
                Object.Destroy(obj);

            if (objects is List<GameObject> listObjects)
                listObjects.Clear();
        }

        public static Vector2 ChangeX(this Vector2 source, float x)
        {
            return new Vector2(x, source.y);
        }
        
        public static Vector2 ChangeY(this Vector2 source, float y)
        {
            return new Vector2(source.x, y);
        }
        
        public static Vector3 To3(this Vector2 source)
        {
            return new Vector3(source.x, source.y, 0f);
        }
        
        public static Vector2 To2(this Vector3 source)
        {
            return new Vector2(source.x, source.y);
        }
        
        public static void ResetLocal(this Transform source)
        {
            source.localPosition = Vector3.zero;
            source.localScale = Vector3.one;
            source.localRotation = Quaternion.identity;
        }
        
        public static void SetLocal(this Transform source, TransformData transformData)
        {
            source.localPosition = transformData.Position;
            source.localScale = transformData.Scale;
            source.localRotation = Quaternion.Euler(transformData.Rotation);
        }

        public static bool HasAndEqual<TKey, TValue>(
            this Dictionary<TKey, TValue> source, 
            TKey checkKey,
            TValue checkValue)
        where TValue : IEquatable<TValue>
        {
            if (!source.TryGetValue(checkKey, out TValue realValue))
                return false;

            return realValue.Equals(checkValue);
        }

        public static bool HasAndEqual<TKey, TValue>(
            this Dictionary<TKey, object> source,
            TKey checkKey,
            TValue checkValue)
        {
            if (!source.TryGetValue(checkKey, out object realValue))
                return false;

            return realValue is TValue tObject && tObject.Equals(checkValue);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        
        public static bool Contain(this RectTransform rectTransform, Vector2 point)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, point, null, out Vector2 localPoint);
            return rectTransform.rect.Contains(localPoint);
        }

        public static void Dispose(this IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
                disposable?.Dispose();
        }

        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.TryAdd(key, value))
                return;
            dict[key] = value;
        }

        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(item => item != null);
        }

        public static void Fill<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
        {
            if (dict.Count == 0)
                return;

            foreach (var key in dict.Keys.ToArray())
                dict[key] = value;
        }

        public static void Fill<TValue>(this TValue[] array, TValue value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
        
        public static void FillInstances<TValue>(this TValue[] array, Func<int, TValue> instantiate)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = instantiate(i);
        }
        
        public static IObservable<Unit> ToObservableOnComplete(this Tween source)
        {
            Subject<Unit> subject = new();
            source.onComplete += () =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            return subject.ObserveOnMainThread();
        }

        public static IObservable<Unit> ToObservableOnKill(this Tween source)
        {
            Subject<Unit> subject = new();
            source.onKill += () =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            return subject.ObserveOnMainThread();
        }

        public static IObservable<Unit> ToObservable(this Tween source)
        {
            Subject<Unit> subject = new();
            source.onKill += () =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            
            source.onComplete += () =>
            {
                subject.OnNext(Unit.Default);
            };
            return subject.ObserveOnMainThread();
        }

        public static Vector2 Sum(this IEnumerable<Vector2> source)
        {
            return source.Aggregate(Vector2.zero, (acc, x) => acc + x);
        }

        public static TweenerCore<Color, Color, ColorOptions> DOColorOutline(this MPImage target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.OutlineColor, x => target.OutlineColor = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        public static TweenerCore<float, float, FloatOptions> DORadius(this MPImage target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(
                () => target.Circle.Radius,
                x =>
                {
                    var circle = target.Circle;
                    circle.Radius = x;
                    target.Circle = circle;
                },
                endValue, 
                duration);
            t.SetTarget(target);
            return t;
        }

        public static IObservable<Vector3> OnPositionChangedAsObservable(this Transform target)
        {
            Vector3 position = target.position;
            Vector3 delta = Vector3.zero;
            
            return Observable.EveryUpdate()
                .Where(_ =>
                {
                    bool isChanged = position != target.position;
                    delta = target.position - position;
                    position = target.position;
                    return isChanged;
                })
                .Select(_ => delta)
                .ObserveOnMainThread();
        }
        
        public static string TruncateWithEllipsis(this string input, int maxLength)
        {
            if (input == null || input.Length <= maxLength) 
                return input;
            return input.Substring(0, maxLength - 3) + "...";
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
        
        public static Vector2 Clamp(this Vector2 source, float min, float max)
        {
            return new Vector2(
                Mathf.Clamp(source.x, min, max), 
                Mathf.Clamp(source.y, min, max));
        }
        
        public static Vector3 Clamp(this Vector3 source, float min, float max)
        {
            return new Vector3(
                Mathf.Clamp(source.x, min, max), 
                Mathf.Clamp(source.y, min, max),
                Mathf.Clamp(source.z, min, max));
        }
        
        public static Vector2 Clamp01(this Vector2 source)
        {
            return source.Clamp(0, 1);
        }
        
        public static Vector3 Clamp01(this Vector3 source)
        {
            return source.Clamp(0, 1);
        }

        public static Vector2 SwapAxes(this Vector2 source)
        {
            return new Vector2(source.y, source.x);
        }

        public static float Clamp(this MinMax source, float value)
        {
            return Mathf.Clamp(value, source.Min, source.Max);
        }
        
        public static float InverseLerp(this MinMax source, float value)
        {
            return Mathf.InverseLerp(source.Min, source.Max, value);
        }
        
        public static float Lerp(this MinMax source, float t)
        {
            return Mathf.Lerp(source.Min, source.Max, t);
        }
        
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}