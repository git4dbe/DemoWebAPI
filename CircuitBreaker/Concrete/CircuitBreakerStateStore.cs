using CircuitBreaker.Contracts;
using CircuitBreaker.Enums;
using System;

namespace CircuitBreaker.Concrete
{
    public class CircuitBreakerStateStore : ICircuitBreakerStateStore
    {
        public CircuitBreakerStateStore()
        {
            Reset();
        }

        public CircuitBreakerStateEnum State { get; private set; }

        public Exception LastException { get; private set; }

        public DateTime LastStateChangedDateUtc { get; private set; }

        public bool IsClosed => State == CircuitBreakerStateEnum.Closed;

        public void HalfOpen()
        {
            State = CircuitBreakerStateEnum.HalfOpen;
        }

        public void Reset()
        {
            State = CircuitBreakerStateEnum.Closed;
        }

        public void Trip(Exception ex)
        {
            //
            // The Trip method switches the state of the circuit breaker to the open state
            // and records the exception that caused the change in state,
            // together with the date and time that the exception occurred.
            //
            State = CircuitBreakerStateEnum.Open;
            LastException = ex;
            LastStateChangedDateUtc = DateTime.UtcNow;
        }
    }
}
