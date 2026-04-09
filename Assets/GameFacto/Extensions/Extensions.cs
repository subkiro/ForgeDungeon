using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public static class Extensions 
{
    #region Checks

    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }
    public static bool IsNull(this object value)
    {
        return value == null;
    }

    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        if (list == null) return true;
        if (list.Count == 0) return true;

        return (list == null) || (list.Count == 0);

    }

    #endregion


    #region Tranforms

    public static RectTransform RectTransform(this Transform trans)
    {
        return trans.GetComponent<RectTransform>();
    }


    public static void ResetTransform(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.rotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }


    public static void ResetLocalTransform(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
    public static void ResetRectTransform(this RectTransform trans)
    {

        trans.anchoredPosition = new Vector2(0, 0);
        trans.anchoredPosition3D = new Vector3(0f, 0f, 0f);
        trans.localPosition = new Vector3(0f, 0f, 0f);
        trans.position = new Vector3(0f, 0f, 0f);


       
    }

    public static void ResetRectOffcets(this RectTransform trans)
    {

        trans.offsetMin = new Vector2(0, 0);
        trans.offsetMax = new Vector2(0, 0);

    }
    public static bool IsRect(this Transform trans)
    {
        return trans.GetType() == typeof(RectTransform);
    }

    public static void CopySizeDelta(this RectTransform To, RectTransform From)
    {
        To.sizeDelta = From.sizeDelta;
    }
    public static void CopyAnchorMinMax(this RectTransform To, RectTransform From)
    {
        To.anchorMin = From.anchorMin;
        To.anchorMax = From.anchorMax;
    }
    public static void CopyAnchorPos(this RectTransform To, RectTransform From)
    {

        To.anchoredPosition = From.anchoredPosition;
    }
    public static void CopyPivot(this RectTransform To, RectTransform From)
    {
        To.pivot = From.pivot;
    }
    public static void CopyPosition(this RectTransform To, RectTransform From)
    {
        To.position = To.position;
    }
    public static void CopyRotation(this RectTransform To, RectTransform From)
    {
        To.rotation = From.rotation;
    }
    public static void CopyScale(this RectTransform To, RectTransform From)
    {
        To.localScale = To.localScale;
    }
    #endregion


    #region AnchorPresets


    public enum AnchorPresets
    {
        None,
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottomCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }
    }

    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {

        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }
    #endregion


    #region Numbers
    public static double RoundToSignificantDigits(this double d, int digits)
    {
        if (d == 0)
            return 0;

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return scale * Math.Round(d / scale, digits);
    }
    #endregion

    #region Enums
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
    #endregion

    #region Awaitable
    public static Awaitable ToAwaitable(this Tween tween, CancellationToken ct = default)
    {
        var tcs = new AwaitableCompletionSource();

        bool completed = false;
        CancellationTokenRegistration ctr = default;

        void Complete()
        {
            if (completed) return;
            completed = true;

            ctr.Dispose();
            tcs.TrySetResult();
        }

        void Cancel()
        {
            if (completed) return;
            completed = true;

            ctr.Dispose();
            tcs.TrySetCanceled();
        }

        tween.OnComplete(Complete);

        tween.OnKill(() =>
        {
            // Called after complete as well → guarded
            Cancel();
        });

        if (ct.CanBeCanceled)
        {
            ctr = ct.Register(() =>
            {
                if (tween.IsActive())
                    tween.Kill();
            });
        }

        return tcs.Awaitable;
    }


    public static class AwaitableUtils
    {
        public static Awaitable WhenAll(params Tween[] tweens)
        {
            return WhenAll(default, tweens);
        }
        public static Awaitable WhenAll(IEnumerable<Awaitable> awaitables)
        {
            var tcs = new AwaitableCompletionSource();

            var list = awaitables.ToList();
            int remaining = list.Count;

            if (remaining == 0)
            {
                tcs.TrySetResult();
                return tcs.Awaitable;
            }

            bool completed = false;

            void TryComplete()
            {
                if (completed) return;
                completed = true;
                tcs.TrySetResult();
            }

            foreach (var a in list)
            {
                a.GetAwaiter().OnCompleted(() =>
                {
                    if (completed) return;

                    remaining--;

                    if (remaining == 0)
                        TryComplete();
                });
            }

            return tcs.Awaitable;
        }
        public static Awaitable WhenAll(CancellationToken ct = default, params Tween[] tweens)
        {
            var tcs = new AwaitableCompletionSource();

            if (tweens == null || tweens.Length == 0)
            {
                tcs.TrySetResult();
                return tcs.Awaitable;
            }

            int remaining = tweens.Length;
            bool completed = false;

            void TryComplete()
            {
                if (completed) return;
                completed = true;
                tcs.TrySetResult();
            }

            void TryCancel()
            {
                if (completed) return;
                completed = true;
                tcs.TrySetCanceled();
            }

            CancellationTokenRegistration ctr = default;

            if (ct.CanBeCanceled)
            {
                ctr = ct.Register(() =>
                {
                    foreach (var t in tweens)
                    {
                        if (t != null && t.IsActive())
                            t.Kill();
                    }

                    TryCancel();
                });
            }

            foreach (var tween in tweens)
            {
                if (tween == null)
                {
                    remaining--;
                    continue;
                }

                tween.ToAwaitable().GetAwaiter().OnCompleted(() =>
                {
                    if (completed) return;

                    remaining--;

                    if (remaining <= 0)
                    {
                        ctr.Dispose();
                        TryComplete();
                    }
                });
            }

            return tcs.Awaitable;
        }
    }


    #endregion

}
