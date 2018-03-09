using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Camera
{
    /// <summary>
    /// Takes in progress which is the percentage of the tween complete and returns
    /// the interpolation value that is fed into the lerp function for the tween.
    /// </summary>
    /// <remarks>
    /// Scale functions are used to define how the tween should occur. Examples would be linear,
    /// easing in quadratic, or easing out circular. You can implement your own scale function
    /// or use one of the many defined in the ScaleFuncs static class.
    /// </remarks>
    /// <param name="progress">The percentage of the tween complete in the range [0, 1].</param>
    /// <returns>The scale value used to lerp between the tween's start and end values</returns>
    public delegate float ScaleFunc(float progress);

    /// <summary>
    /// Standard linear interpolation function: "start + (end - start) * progress"
    /// </summary>
    /// <remarks>
    /// In a language like C++ we wouldn't need this delegate at all. Templates in C++ would allow us
    /// to simply write "start + (end - start) * progress" in the tween class and the compiler would
    /// take care of enforcing that the type supported those operators. Unfortunately C#'s generics
    /// are not so powerful so instead we must have the user provide the interpolation function.
    ///
    /// Thankfully frameworks like XNA and Unity provide lerp functions on their primitive math types
    /// which means that for most users there is nothing specific to do here. Additionally this file
    /// provides concrete implementations of tweens for vectors, colors, and more for XNA and Unity
    /// users, lessening the burden even more.
    /// </remarks>
    /// <typeparam name="T">The type to interpolate.</typeparam>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <param name="progress">The interpolation progress.</param>
    /// <returns>The interpolated value, generally using "start + (end - start) * progress"</returns>
    public delegate T LerpFunc<T>(T start, T end, float progress);

    /// <summary>
    /// State of an ITween object
    /// </summary>
    public enum TweenState
    {
        /// <summary>
        /// The tween is running.
        /// </summary>
        Running,

        /// <summary>
        /// The tween is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The tween is stopped.
        /// </summary>
        Stopped
    }

    /// <summary>
    /// The behavior to use when manually stopping a tween.
    /// </summary>
    public enum StopBehavior
    {
        /// <summary>
        /// Does not change the current value.
        /// </summary>
        AsIs,

        /// <summary>
        /// Forces the tween progress to the end value.
        /// </summary>
        ForceComplete
    }

    /// <summary>
    /// Interface for a tween object.
    /// </summary>
    public interface ITween
    {
        /// <summary>
        /// Gets the current state of the tween.
        /// </summary>
        TweenState State { get; }

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the paused tween.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stops the tween.
        /// </summary>
        /// <param name="stopBehavior">The behavior to use to handle the stop.</param>
        void Stop(StopBehavior stopBehavior);

        /// <summary>
        /// Updates the tween.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time to add to the tween.</param>
        void Update(float elapsedTime);
    }

    /// <summary>
    /// Interface for a tween object that handles a specific type.
    /// </summary>
    /// <typeparam name="T">The type to tween.</typeparam>
    public interface ITween<T> : ITween
        where T : struct
    {
        /// <summary>
        /// Gets the current value of the tween.
        /// </summary>
        T CurrentValue { get; }

        /// <summary>
        /// Starts a tween.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="scaleFunc">A function used to scale progress over time.</param>
        void Start(T start, T end, float duration, ScaleFunc scaleFunc);
    }

    /// <summary>
    /// A concrete implementation of a tween object.
    /// </summary>
    /// <typeparam name="T">The type to tween.</typeparam>
    public class Tween<T> : ITween<T>
        where T : struct
    {
        public delegate Task OnCompleted();
        public event OnCompleted Completed;


        private readonly LerpFunc<T> lerpFunc;

        private ScaleFunc scaleFunc;

        /// <summary>
        /// Gets the current time of the tween.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// Gets the duration of the tween.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Gets the current state of the tween.
        /// </summary>
        public TweenState State { get; private set; }

        /// <summary>
        /// Gets the starting value of the tween.
        /// </summary>
        public T StartValue { get; private set; }

        /// <summary>
        /// Gets the ending value of the tween.
        /// </summary>
        public T EndValue { get; private set; }

        /// <summary>
        /// Gets the current value of the tween.
        /// </summary>
        public T CurrentValue { get; private set; }

        /// <summary>
        /// Initializes a new Tween with a given lerp function.
        /// </summary>
        /// <remarks>
        /// C# generics are good but not good enough. We need a delegate to know how to
        /// interpolate between the start and end values for the given type.
        /// </remarks>
        /// <param name="lerpFunc">The interpolation function for the tween type.</param>
        public Tween(LerpFunc<T> lerpFunc)
        {
            this.lerpFunc = lerpFunc;
            State = TweenState.Stopped;
        }

        /// <summary>
        /// Starts a tween.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="scaleFunc">A function used to scale progress over time.</param>
        public void Start(T start, T end, float duration, ScaleFunc scaleFunc)
        {
            if (duration <= 0)
            {
                throw new ArgumentException("duration must be greater than 0");
            }

            CurrentTime = 0;
            this.Duration = duration;
            this.scaleFunc = scaleFunc ?? throw new ArgumentNullException("scaleFunc");
            State = TweenState.Running;

            this.StartValue = start;
            this.EndValue = end;

            UpdateValue();
        }

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        public void Pause()
        {
            if (State == TweenState.Running)
            {
                State = TweenState.Paused;
            }
        }

        /// <summary>
        /// Resumes the paused tween.
        /// </summary>
        public void Resume()
        {
            if (State == TweenState.Paused)
            {
                State = TweenState.Running;
            }
        }

        /// <summary>
        /// Stops the tween.
        /// </summary>
        /// <param name="stopBehavior">The behavior to use to handle the stop.</param>
        public void Stop(StopBehavior stopBehavior)
        {
            State = TweenState.Stopped;

            if (stopBehavior == StopBehavior.ForceComplete)
            {
                CurrentTime = Duration;
                UpdateValue();
            }

            Completed?.Invoke();
        }

        /// <summary>
        /// Updates the tween.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time to add to the tween.</param>
        public async void Update(float elapsedTime)
        {
            if (State != TweenState.Running)
            {
                return;
            }

            CurrentTime += elapsedTime;

            if (CurrentTime >= Duration)
            {
                CurrentTime = Duration;
                State = TweenState.Stopped;
                await Completed?.Invoke();
            }

            UpdateValue();
        }

        /// <summary>
        /// Helper that uses the current time, duration, and delegates to update the current value.
        /// </summary>
        private void UpdateValue()
        {
            CurrentValue = lerpFunc(StartValue, EndValue, scaleFunc(CurrentTime / Duration));
        }
    }

    /// <summary>
    /// Object used to tween float values.
    /// </summary>
    public class FloatTween : Tween<float>
    {
        private static float LerpFloat(float start, float end, float progress) { return start + (end - start) * progress; }

        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<float> LerpFunc = LerpFloat;

        /// <summary>
        /// Initializes a new FloatTween instance.
        /// </summary>
        public FloatTween() : base(LerpFunc) { }
    }

    /// <summary>
    /// Defines a set of premade scale functions for use with tweens.
    /// </summary>
    /// <remarks>
    /// To avoid excess allocations of delegates, the public members of ScaleFuncs are already
    /// delegates that reference private methods.
    ///
    /// Implementations based on http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c
    /// which are based on http://www.robertpenner.com/easing/
    /// </remarks>
    public static class ScaleFuncs
    {
        /// <summary>
        /// A linear progress scale function.
        /// </summary>
        public static readonly ScaleFunc Linear = LinearImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseIn = QuadraticEaseInImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseOut = QuadraticEaseOutImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseInOut = QuadraticEaseInOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc CubicEaseIn = CubicEaseInImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc CubicEaseOut = CubicEaseOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc CubicEaseInOut = CubicEaseInOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseIn = QuarticEaseInImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseOut = QuarticEaseOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseInOut = QuarticEaseInOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseIn = QuinticEaseInImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseOut = QuinticEaseOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseInOut = QuinticEaseInOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc SineEaseIn = SineEaseInImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc SineEaseOut = SineEaseOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc SineEaseInOut = SineEaseInOutImpl;

        private const float Pi = (float)Math.PI;
        private const float HalfPi = Pi / 2f;

        private static float LinearImpl(float progress) { return progress; }
        private static float QuadraticEaseInImpl(float progress) { return EaseInPower(progress, 2); }
        private static float QuadraticEaseOutImpl(float progress) { return EaseOutPower(progress, 2); }
        private static float QuadraticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 2); }
        private static float CubicEaseInImpl(float progress) { return EaseInPower(progress, 3); }
        private static float CubicEaseOutImpl(float progress) { return EaseOutPower(progress, 3); }
        private static float CubicEaseInOutImpl(float progress) { return EaseInOutPower(progress, 3); }
        private static float QuarticEaseInImpl(float progress) { return EaseInPower(progress, 4); }
        private static float QuarticEaseOutImpl(float progress) { return EaseOutPower(progress, 4); }
        private static float QuarticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 4); }
        private static float QuinticEaseInImpl(float progress) { return EaseInPower(progress, 5); }
        private static float QuinticEaseOutImpl(float progress) { return EaseOutPower(progress, 5); }
        private static float QuinticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 5); }

        private static float EaseInPower(float progress, int power)
        {
            return (float)Math.Pow(progress, power);
        }

        private static float EaseOutPower(float progress, int power)
        {
            int sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1, power) + sign));
        }

        private static float EaseInOutPower(float progress, int power)
        {
            progress *= 2;
            if (progress < 1)
            {
                return (float)Math.Pow(progress, power) / 2f;
            }
            else
            {
                int sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(progress - 2, power) + sign * 2));
            }
        }

        private static float SineEaseInImpl(float progress)
        {
            return (float)Math.Sin(progress * HalfPi - HalfPi) + 1;
        }

        private static float SineEaseOutImpl(float progress)
        {
            return (float)Math.Sin(progress * HalfPi);
        }

        private static float SineEaseInOutImpl(float progress)
        {
            return (float)(Math.Sin(progress * Pi - HalfPi) + 1) / 2;
        }
    }
}
