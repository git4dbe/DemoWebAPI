using CircuitBreaker.Exceptions;
using System;
using System.Threading;

namespace CircuitBreaker.Concrete
{
    //
    // https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker
    //
    public class CircuitBreakerFunc<T> 
    {
        public CircuitBreakerState BreakerState { get; private set; }
        public CircuitBreakerFunc(CircuitBreakerState breakerState)
        {
            BreakerState = breakerState;
        }

        public T ExecuteFunc(Func<T> func)
        {
            if (BreakerState.IsOpen)
            {
                return ExecuteFuncInOpenMode(func);
            }

            // The circuit breaker is Closed, execute the action.
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                // If an exception still occurs here, simply
                // retrip the breaker immediately.
                BreakerState.TrackException(ex);

                // Throw the exception so that the caller can tell
                // the type of exception that was thrown.
                throw;
            }
        }

        private T ExecuteFuncInOpenMode(Func<T> func)
        {
            // The circuit breaker is Open. Check if the Open timeout has expired.
            // If it has, set the state to HalfOpen. Another approach might be to
            // check for the HalfOpen state that had be set by some other operation.
            if (BreakerState.StateStore.LastStateChangedDateUtc + BreakerState.OpenToHalfOpenWaitTime < DateTime.UtcNow)
            {
                // The Open timeout has expired. Allow one operation to execute. Note that, in
                // this example, the circuit breaker is set to HalfOpen after being
                // in the Open state for some period of time. An alternative would be to set
                // this using some other approach such as a timer, test method, manually, and
                // so on, and check the state here to determine how to handle execution
                // of the action.
                // Limit the number of threads to be executed when the breaker is HalfOpen.
                // An alternative would be to use a more complex approach to determine which
                // threads or how many are allowed to execute, or to execute a simple test
                // method instead.
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(BreakerState.HalfOpenSyncObject, ref lockTaken);
                    if (lockTaken)
                    {
                        // Set the circuit breaker state to HalfOpen.
                        BreakerState.StateStore.HalfOpen();

                        // Attempt the operation.
                        return func();
                    }
                }
                catch (Exception ex)
                {
                    // If there's still an exception, trip the breaker again immediately.
                    BreakerState.StateStore.Trip(ex);

                    // Throw the exception so that the caller knows which exception occurred.
                    throw;
                }
                finally
                {
                    if (lockTaken)
                    {
                        BreakerState.StateStore.Reset();
                        Monitor.Exit(BreakerState.HalfOpenSyncObject);
                    }
                }
            }
            // The Open timeout hasn't yet expired. Throw a CircuitBreakerOpen exception to
            // inform the caller that the call was not actually attempted,
            // and return the most recent exception received.
            throw new CircuitBreakerOpenException(BreakerState.StateStore.LastException);
        }
    }
}
