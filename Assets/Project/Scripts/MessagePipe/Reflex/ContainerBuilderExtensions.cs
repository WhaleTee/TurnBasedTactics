using System;
using MessagePipe;
using Reflex.Core;
using Reflex.Enums;

namespace WhaleTee.MessagePipe.Reflex {
  public static class ContainerBuilderExtensions {
    public static MessagePipeOptions RegisterMessagePipe(this ContainerBuilder containerBuilder) {
      return RegisterMessagePipe(containerBuilder, _ => { });
    }

    public static MessagePipeOptions RegisterMessagePipe(this ContainerBuilder containerBuilder, Action<MessagePipeOptions> configure) {
      MessagePipeOptions messagePipeOptions = null;
      var proxy = new ServiceCollectionProxy(containerBuilder);

      proxy.AddMessagePipe(options => {
                             configure(options);
                             messagePipeOptions = options;
                           }
      );

      containerBuilder.AddScoped(container => new ServiceProviderProxy(container), typeof(IServiceProvider));

      return messagePipeOptions;
    }

    public static ContainerBuilder RegisterMessageBroker<TMessage>(
      this ContainerBuilder builder,
      Lifetime lifetime = Lifetime.Singleton
    ) {
      var services = new ServiceCollectionProxy(builder);

      // keyless Pub/Sub
      services.Add(typeof(MessageBrokerCore<TMessage>), lifetime);
      services.Add(typeof(IPublisher<TMessage>), typeof(MessageBroker<TMessage>), lifetime);
      services.Add(typeof(ISubscriber<TMessage>), typeof(MessageBroker<TMessage>), lifetime);

      // keyless Pub/Sub async
      services.Add(typeof(AsyncMessageBrokerCore<TMessage>), lifetime);
      services.Add(typeof(IAsyncPublisher<TMessage>), typeof(AsyncMessageBroker<TMessage>), lifetime);
      services.Add(typeof(IAsyncSubscriber<TMessage>), typeof(AsyncMessageBroker<TMessage>), lifetime);

      // keyless buffered Pub/Sub
      services.Add(typeof(BufferedMessageBrokerCore<TMessage>), lifetime);
      services.Add(typeof(IBufferedPublisher<TMessage>), typeof(BufferedMessageBroker<TMessage>), lifetime);
      services.Add(typeof(IBufferedSubscriber<TMessage>), typeof(BufferedMessageBroker<TMessage>), lifetime);

      // keyless buffered Pub/Sub async
      services.Add(typeof(BufferedAsyncMessageBrokerCore<TMessage>), lifetime);
      services.Add(typeof(IBufferedAsyncPublisher<TMessage>), typeof(BufferedAsyncMessageBroker<TMessage>), lifetime);
      services.Add(typeof(IBufferedAsyncSubscriber<TMessage>), typeof(BufferedAsyncMessageBroker<TMessage>), lifetime);

      return builder;
    }

    public static ContainerBuilder RegisterMessageBroker<TKey, TMessage>(
      this ContainerBuilder containerBuilder,
      Lifetime lifetime = Lifetime.Singleton
    ) {
      var services = new ServiceCollectionProxy(containerBuilder);

      // keyed Pub/Sub
      services.Add(typeof(MessageBrokerCore<TKey, TMessage>), lifetime);
      services.Add(typeof(IPublisher<TKey, TMessage>), typeof(MessageBroker<TKey, TMessage>), lifetime);
      services.Add(typeof(ISubscriber<TKey, TMessage>), typeof(MessageBroker<TKey, TMessage>), lifetime);

      // keyed Pub/Sub async
      services.Add(typeof(AsyncMessageBrokerCore<TKey, TMessage>), lifetime);
      services.Add(typeof(IAsyncPublisher<TKey, TMessage>), typeof(AsyncMessageBroker<TKey, TMessage>), lifetime);
      services.Add(typeof(IAsyncSubscriber<TKey, TMessage>), typeof(AsyncMessageBroker<TKey, TMessage>), lifetime);

      return containerBuilder;
    }

    public static ContainerBuilder RegisterRequestHandler<TRequest, TResponse, THandler>(
      this ContainerBuilder containerBuilder,
      Lifetime lifetime = Lifetime.Singleton
    ) where THandler : IRequestHandler {
      var services = new ServiceCollectionProxy(containerBuilder);

      services.Add(typeof(IRequestHandlerCore<TRequest, TResponse>), typeof(THandler), lifetime);

      if (!containerBuilder.HasBinding(typeof(IRequestHandler<TRequest, TResponse>))) {
        services.Add(typeof(IRequestHandler<TRequest, TResponse>), typeof(RequestHandler<TRequest, TResponse>), lifetime);
        services.Add(typeof(IRequestAllHandler<TRequest, TResponse>), typeof(RequestAllHandler<TRequest, TResponse>), lifetime);
      }

      return containerBuilder;
    }

    public static ContainerBuilder RegisterAsyncRequestHandler<TRequest, TResponse, THandler>(
      this ContainerBuilder containerBuilder,
      Lifetime lifetime = Lifetime.Singleton
    ) where THandler : IAsyncRequestHandler {
      var services = new ServiceCollectionProxy(containerBuilder);

      services.Add(typeof(IAsyncRequestHandlerCore<TRequest, TResponse>), typeof(THandler), lifetime);

      if (!containerBuilder.HasBinding(typeof(IAsyncRequestHandler<TRequest, TResponse>))) {
        services.Add(typeof(IAsyncRequestHandler<TRequest, TResponse>), typeof(AsyncRequestHandler<TRequest, TResponse>), lifetime);
        services.Add(typeof(IAsyncRequestAllHandler<TRequest, TResponse>), typeof(AsyncRequestAllHandler<TRequest, TResponse>), lifetime);
      }

      AsyncRequestHandlerRegistory.Add(typeof(TRequest), typeof(TResponse), typeof(THandler));
      return containerBuilder;
    }

    public static ContainerBuilder RegisterMessageHandlerFilter<T>(this ContainerBuilder containerBuilder) where T : class, IMessageHandlerFilter {
      var type = typeof(T);
      if (!containerBuilder.HasBinding(type)) containerBuilder.AddTransient(type);
      return containerBuilder;
    }

    public static ContainerBuilder RegisterAsyncMessageHandlerFilter<T>(this ContainerBuilder containerBuilder) where T : class, IAsyncMessageHandlerFilter {
      var type = typeof(T);
      if (!containerBuilder.HasBinding(type)) containerBuilder.AddTransient(type);
      return containerBuilder;
    }

    public static ContainerBuilder RegisterRequestHandlerFilter<T>(this ContainerBuilder containerBuilder) where T : class, IRequestHandlerFilter {
      var type = typeof(T);
      if (!containerBuilder.HasBinding(type)) containerBuilder.AddTransient(type);
      return containerBuilder;
    }

    public static ContainerBuilder RegisterAsyncRequestHandlerFilter<T>(this ContainerBuilder containerBuilder) where T : class, IAsyncRequestHandlerFilter {
      var type = typeof(T);
      if (!containerBuilder.HasBinding(type)) containerBuilder.AddTransient(type);
      return containerBuilder;
    }

    public static IServiceCollection AsServiceCollection(this ContainerBuilder builder) {
      return new ServiceCollectionProxy(builder);
    }

    public static IMessagePipeBuilder ToMessagePipeBuilder(this ContainerBuilder builder) {
      return new MessagePipeBuilder(builder.AsServiceCollection());
    }
  }
}