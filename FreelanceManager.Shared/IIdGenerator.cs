using System;
namespace FreelanceManager
{
    public interface IIdGenerator
    {
        Guid NextGuid();
        string Next();
    }
}
