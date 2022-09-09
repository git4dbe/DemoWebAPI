using CircuitBreaker.Exceptions;
using System;
using System.Threading;

namespace CircuitBreaker.Concrete
{
    //
    // https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker
    //
    public class CircuitBreakerAction 
    {
        public CircuitBreakerState BreakerState { get; private set; }


        public CircuitBreakerAction(CircuitBreakerState breakerState) 
        {
            BreakerState = breakerState;
        }

        public void ExecuteAction(Action action)
        {
            if (BreakerState.IsOpen)
            {
                ExecuteActionInOpenMode(action);
            }

            // The circuit breaker is Closed, execute the action.
            try
            {
                action();
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

        private void ExecuteActionInOpenMode(Action action)
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
                        action();

                        // If this action succeeds, reset the state and allow other operations.
                        // In reality, instead of immediately returning to the Closed state, a counter
                        // here would record the number of successful operations and return the
                        // circuit breaker to the Closed state only after a specified number succeed.
                        BreakerState.StateStore.Reset();
                        return;
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
