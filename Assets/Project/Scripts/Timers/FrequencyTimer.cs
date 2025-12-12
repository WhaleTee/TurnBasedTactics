using System;
using UnityEngine;

namespace WhaleTee.Timers {
  public class FrequencyTimer : Timer {
    public int TicksPerSecond { get; private set; }

    public readonly Action onTick = delegate { };

    float timeThreshold;

    public FrequencyTimer(int ticksPerSecond) : base(0) {
      CalculateTimeThreshold(ticksPerSecond);
    }

    public override void Tick() {
      if (IsRunning && CurrentTime >= timeThreshold) {
        CurrentTime -= timeThreshold;
        onTick.Invoke();
      }

      if (IsRunning && CurrentTime < timeThreshold) CurrentTime += Time.deltaTime;
    }

    public override bool IsFinished => !IsRunning;

    public override void Reset() {
      CurrentTime = 0;
    }

    public void Reset(int newTicksPerSecond) {
      CalculateTimeThreshold(newTicksPerSecond);
      Reset();
    }

    void CalculateTimeThreshold(int ticksPerSecond) {
      TicksPerSecond = ticksPerSecond;
      timeThreshold = 1f / TicksPerSecond;
    }
  }
}