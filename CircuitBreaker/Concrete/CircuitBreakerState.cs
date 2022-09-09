using CircuitBreaker.Contracts;
using System;

namespace CircuitBreaker.Concrete
{
    //
    // https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker
    //
    public class CircuitBreakerState
    {
        public ICircuitBreakerStateStore StateStore { get; private set; }
        public object HalfOpenSyncObject { get; private set; }

        public bool IsClosed => StateStore.IsClosed;
        public bool IsOpen => !IsClosed;

        public TimeSpan OpenToHalfOpenWaitTime { get; private set; }

        public CircuitBreakerState(int openToHalfOpenWaitTime)
        {
            StateStore = new CircuitBreakerStateStore();
            HalfOpenSyncObject = new object();
            OpenToHalfOpenWaitTime = TimeSpan.FromMilliseconds(openToHalfOpenWaitTime);
        }

        public void TrackException(Exception ex)
        {
            // For simplicity in this example, open the circuit breaker on the first exception.
            // In reality this would be more complex. A certain type of exception, such as one
            // that indicates a service is offline, might trip the circuit breaker immediately.
            // Alternatively it might count exceptions locally or across multiple instances and
            // use this value over time, or the exception/success ratio based on the exception
            // types, to open the circuit breaker.
            StateStore.Trip(ex);
        }

    }
}
