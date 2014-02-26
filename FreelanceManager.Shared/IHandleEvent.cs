namespace FreelanceManager
{
    public interface IHandleEvent<T>
    {
        void Handle(T @event);
    }
}
