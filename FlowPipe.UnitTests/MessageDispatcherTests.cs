using FlowPipe.Contracts;
using FlowPipe.Decorators;
using Moq;

namespace FlowPipe.UnitTests;

public class MessageDispatcherTests
{
    [Fact]
    public async Task SendAsync_Should_Call_Handler_When_No_Behavior()
    {
        // Arrange
        var ping = new Ping();

        // Mock the handler
        var handlerMock = new Mock<IMessageHandler<Ping, string>>();
        handlerMock
            .Setup(h => h.HandleAsync(ping, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Pong");

        // Mock the service provider
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Setup GetService for the handler
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IMessageHandler<Ping, string>)))
            .Returns(handlerMock.Object);

        // Setup GetService for the behaviors (none)
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IMessageBehavior<Ping, string>>)))
            .Returns(Enumerable.Empty<IMessageBehavior<Ping, string>>());

        var dispatcher = new MessageDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.SendAsync(ping);

        // Assert
        Assert.Equal("Pong", result);
        handlerMock.Verify(h => h.HandleAsync(ping, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_Invoke_Behaviors_In_Correct_Order()
    {
        // Arrange
        var ping = new Ping();

        var handlerMock = new Mock<IMessageHandler<Ping, string>>();
        handlerMock
            .Setup(h => h.HandleAsync(ping, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Handler");

        var behavior1Mock = new Mock<IMessageBehavior<Ping, string>>();
        behavior1Mock.SetupGet(b => b.BehaviorSequence).Returns(1);
        behavior1Mock
            .Setup(b => b.HandleAsync(
                ping,
                It.IsAny<MessageHandlerDelegate<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Ping, MessageHandlerDelegate<string>, CancellationToken>((_, next, _) =>
                Task.FromResult("B1(" + next().Result + ")"));

        var behavior2Mock = new Mock<IMessageBehavior<Ping, string>>();
        behavior2Mock.SetupGet(b => b.BehaviorSequence).Returns(2);
        behavior2Mock
            .Setup(b => b.HandleAsync(
                ping,
                It.IsAny<MessageHandlerDelegate<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Ping, MessageHandlerDelegate<string>, CancellationToken>((_, next, _) =>
                Task.FromResult("B2(" + next().Result + ")"));

        var serviceProviderMock = new Mock<IServiceProvider>();

        // Extension method olan GetRequiredService yerine doğrudan GetService kullandık
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IMessageHandler<Ping, string>)))
            .Returns(handlerMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IMessageBehavior<Ping, string>>)))
            .Returns(new[] { behavior1Mock.Object, behavior2Mock.Object });

        var dispatcher = new MessageDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.SendAsync(ping);

        // Assert
        Assert.Equal("B1(B2(Handler))", result);

        handlerMock.Verify(h => h.HandleAsync(ping, It.IsAny<CancellationToken>()), Times.Once);
        behavior1Mock.Verify(
            b => b.HandleAsync(ping, It.IsAny<MessageHandlerDelegate<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        behavior2Mock.Verify(
            b => b.HandleAsync(ping, It.IsAny<MessageHandlerDelegate<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}