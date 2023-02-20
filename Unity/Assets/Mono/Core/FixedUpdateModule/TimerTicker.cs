// Code from Xenko TimerTick
using System;
using System.Diagnostics;
namespace ET {
    public class TimerTick {
#region Fields
        private long startRawTime;
        private long lastRawTime;
        private int pauseCount;
        private long pauseStartTime;
        private long timePaused;
        private decimal speedFactor;
#endregion
#region Constructors and Destructors
        // Initializes a new instance of the <see cref="TimerTick"/> class.
        public TimerTick() {
            speedFactor = 1.0m;
            Reset();
        }
        // Initializes a new instance of the <see cref="TimerTick" /> class.
        // <param name="startTime">The start time.</param>
        public TimerTick(TimeSpan startTime) {
            speedFactor = 1.0m;
            Reset(startTime);
        }
#endregion
#region Public Properties
        // Gets the start time when this timer was created.
        public TimeSpan StartTime { get; private set; }
        // Gets the total time elasped since the last reset or when this timer was created.
        public TimeSpan TotalTime { get; private set; }
        // Gets the total time elasped since the last reset or when this timer was created, including <see cref="Pause"/>
        public TimeSpan TotalTimeWithPause { get; private set; }
        // Gets the elapsed time since the previous call to <see cref="Tick"/>.
        public TimeSpan ElapsedTime { get; private set; }
        // Gets the elapsed time since the previous call to <see cref="Tick"/> including <see cref="Pause"/>
        public TimeSpan ElapsedTimeWithPause { get; private set; }
        // Gets or sets the speed factor. Default is 1.0
        // <value>The speed factor.</value>
        public double SpeedFactor {
            get { return (double) speedFactor; }
            set { speedFactor = (decimal) value; }
        }
        // Gets a value indicating whether this instance is paused.
        // <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
        public bool IsPaused {
            get { return pauseCount > 0; }
        }
#endregion
#region Public Methods and Operators
        // Resets this instance. <see cref="TotalTime"/> is set to zero.
        public void Reset() {
            Reset(TimeSpan.Zero);
        }
        // Resets this instance. <see cref="TotalTime" /> is set to startTime.
        // <param name="startTime">The start time.</param>
        public void Reset(TimeSpan startTime) {
            StartTime = startTime;
            TotalTime = startTime;
            startRawTime = Stopwatch.GetTimestamp();
            lastRawTime = startRawTime;
            timePaused = 0;
            pauseStartTime = 0;
            pauseCount = 0;
        }
        // Resumes this instance, only if a call to <see cref="Pause"/> has been already issued.
        public void Resume() {
            pauseCount--;
            if (pauseCount <= 0) {
                timePaused += Stopwatch.GetTimestamp() - pauseStartTime;
                pauseStartTime = 0L;
            }
        }
        // Update the <see cref="TotalTime"/> and <see cref="ElapsedTime"/>,
        // <remarks>
        // This method must be called on a regular basis at every *tick*.
        // </remarks>
        public void Tick() {
            // Don't tick when this instance is paused.
            if (IsPaused) {
                ElapsedTime = TimeSpan.Zero;
                return;
            }
            var rawTime = Stopwatch.GetTimestamp();
            TotalTime = StartTime + new TimeSpan((long) Math.Round(ConvertRawToTimestamp(rawTime - timePaused - startRawTime).Ticks * speedFactor));
            TotalTimeWithPause = StartTime + new TimeSpan((long) Math.Round(ConvertRawToTimestamp(rawTime - startRawTime).Ticks * speedFactor));
            ElapsedTime = ConvertRawToTimestamp(rawTime - timePaused - lastRawTime);
            ElapsedTimeWithPause = ConvertRawToTimestamp(rawTime - lastRawTime);
            if (ElapsedTime < TimeSpan.Zero) {
                ElapsedTime = TimeSpan.Zero;
            }
            lastRawTime = rawTime;
        }
        // Pauses this instance.
        public void Pause() {
            pauseCount++;
            if (pauseCount == 1) {
                pauseStartTime = Stopwatch.GetTimestamp();
            }
        }
        public static TimeSpan ConvertRawToTimestamp(long delta) {
            return new TimeSpan(delta == 0 ? 0 : (delta * TimeSpan.TicksPerSecond) / Stopwatch.Frequency);
        }
#endregion
    }
}