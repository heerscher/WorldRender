using System;

namespace WorldRender.Timing
{
    /// <summary>
    /// High resolution timer capable of returning delta time.
    /// </summary>
    public class Timer
    {
        private long lastTimestamp;
        private long nowTimestamp;
        private long frequency;
        private float deltaTime;

        public Timer()
        {
            lastTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            nowTimestamp = lastTimestamp;
            frequency = System.Diagnostics.Stopwatch.Frequency;
        }

        /// <summary>
        /// Returns the time elapsed in seconds between calls to Delta().
        /// </summary>
        public float Delta()
        {
            nowTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            deltaTime = Convert.ToSingle(nowTimestamp - lastTimestamp) / Convert.ToSingle(frequency);
            lastTimestamp = nowTimestamp;

            return deltaTime;
        }
    }
}
