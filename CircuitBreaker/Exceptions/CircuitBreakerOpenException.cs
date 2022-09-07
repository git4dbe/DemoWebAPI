using System;

namespace CircuitBreaker.Exceptions
{
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(Exception ex) : base ("Circuit Breaker Open Exception", ex)
        {
        }
    }
}
