using MessagePipe;
using Reflex.Core;
using WhaleTee.MessagePipe.Key;
using WhaleTee.MessagePipe.Message;

namespace WhaleTee.MessagePipe.Reflex {
  public sealed class MessagePipeContainerDecorator {
    const string MESSAGE_PIPE_CONTAINER_NAME = "Massage Pipe Container";

    readonly MessagePipeOptions options;
    readonly ContainerBuilder containerBuilder;

    public MessagePipeContainerDecorator(ContainerBuilder containerBuilder) {
      this.containerBuilder = containerBuilder;
      var messagePipeContainerBuilder = new ContainerBuilder();
      messagePipeContainerBuilder.SetName(MESSAGE_PIPE_CONTAINER_NAME);
      options = messagePipeContainerBuilder.RegisterMessagePipe();
      foreach (var binding in messagePipeContainerBuilder.Bindings) containerBuilder.Bindings.Add(binding);
      containerBuilder.AddSingleton(_ => messagePipeContainerBuilder.Build().AsServiceProvider());
      containerBuilder.OnContainerBuilt += container => GlobalMessagePipe.SetProvider(container.AsServiceProvider());
    }

    public ContainerBuilder RegisterMessageBroker<TMessage>() where TMessage : IEventMessage {
      return containerBuilder.RegisterMessageBroker<TMessage>(options);
    }

    public ContainerBuilder RegisterMessageBroker<TKey, TMessage>() where TKey : IEventKey where TMessage : IEventMessage {
      return containerBuilder.RegisterMessageBroker<TKey, TMessage>(options);
    }

    public ContainerBuilder RegisterRequestHandler<TRequest, TResponse, THandler>()
    where TRequest : IEventMessage where TResponse : IEventMessage where THandler : IRequestHandler {
      return containerBuilder.RegisterRequestHandler<TRequest, TResponse, THandler>(options);
    }

    public ContainerBuilder RegisterAsyncRequestHandler<TRequest, TResponse, THandler>()
    where TRequest : IEventMessage where TResponse : IEventMessage where THandler : IAsyncRequestHandler {
      return containerBuilder.RegisterAsyncRequestHandler<TRequest, TResponse, THandler>(options);
    }
  }
}