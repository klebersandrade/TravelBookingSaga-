namespace TravelBooking.Common.Saga;
public interface ISagaStep
{
    string StepName { get; }
    SagaState CompletedState { get; }
    SagaState FailedState { get; }
}
