using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhaleTee.Extensions {
  public static class ColorExtensions {
    public static Color With(
      this Color color, float? r = null, float? g = null, float? b = null,
      float? a = null
    ) {
      return new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
    }
  }

  public static class UIEasingModeExtensions {
    public static Ease ToPrimeTweenEase(this EasingMode ease) {
      return ease switch {
               EasingMode.Ease => Ease.Linear,
               EasingMode.EaseIn => Ease.InSine,
               EasingMode.EaseOut => Ease.OutSine,
               EasingMode.EaseInOut => Ease.InOutSine,
               EasingMode.Linear => Ease.Linear,
               EasingMode.EaseInSine => Ease.InSine,
               EasingMode.EaseOutSine => Ease.OutSine,
               EasingMode.EaseInOutSine => Ease.InOutSine,
               EasingMode.EaseInCubic => Ease.InCubic,
               EasingMode.EaseOutCubic => Ease.OutCubic,
               EasingMode.EaseInOutCubic => Ease.InOutCubic,
               EasingMode.EaseInCirc => Ease.InCirc,
               EasingMode.EaseOutCirc => Ease.OutCirc,
               EasingMode.EaseInOutCirc => Ease.InOutCirc,
               EasingMode.EaseInElastic => Ease.InElastic,
               EasingMode.EaseOutElastic => Ease.OutElastic,
               EasingMode.EaseInOutElastic => Ease.InOutElastic,
               EasingMode.EaseInBack => Ease.InBack,
               EasingMode.EaseOutBack => Ease.OutBack,
               EasingMode.EaseInOutBack => Ease.InOutBack,
               EasingMode.EaseInBounce => Ease.InBounce,
               EasingMode.EaseOutBounce => Ease.OutBounce,
               EasingMode.EaseInOutBounce => Ease.InOutBounce,
               var _ => throw new ArgumentOutOfRangeException(nameof(ease), ease, null)
             };
    }
  }
}