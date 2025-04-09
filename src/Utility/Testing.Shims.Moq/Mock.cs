using NSubstitute;

namespace Moq;

public class Mock<T>
    where T : class
{
    public Mock()
    {
        Object = Substitute.For<T>();
    }
    public T Object { get; }
}