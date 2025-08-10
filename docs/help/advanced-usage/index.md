---
title: Advanced usage patterns
---

This guide covers advanced techniques and patterns for using NSubstitute effectively in complex scenarios.

## Complex argument matching

### Custom argument matchers

Create reusable custom matchers for complex business logic:

```csharp
public static class CustomArgs
{
    public static ref T IsValidEmail<T>() where T : class
    {
        return ref Arg.Is<T>(email => 
            email is string str && 
            str.Contains("@") && 
            str.Contains("."));
    }
    
    public static ref T HasProperty<T>(string propertyName, object expectedValue) where T : class
    {
        return ref Arg.Is<T>(obj =>
        {
            var property = obj?.GetType().GetProperty(propertyName);
            return property?.GetValue(obj)?.Equals(expectedValue) == true;
        });
    }
}

// Usage:
userService.SendEmail(CustomArgs.IsValidEmail<string>()).Returns(true);
repository.Save(CustomArgs.HasProperty<User>("IsActive", true)).Returns(Task.CompletedTask);
```

### Combining argument matchers

Use multiple matchers together for complex scenarios:

```csharp
// Match objects with specific combinations of properties
dataService.Process(
    Arg.Is<Data>(d => d.Type == DataType.Important),
    Arg.Is<ProcessingOptions>(o => o.Priority > 5 && o.RetryCount <= 3)
).Returns(ProcessingResult.Success);

// Match collections with specific characteristics
repository.SaveBatch(
    Arg.Is<IEnumerable<Entity>>(entities => 
        entities.Count() <= 100 && 
        entities.All(e => e.IsValid))
).Returns(Task.CompletedTask);
```

## Dynamic behavior patterns

### State-based substitutes

Create substitutes that maintain state and change behavior over time:

```csharp
public class StatefulRepository
{
    private readonly IRepository _repository;
    private readonly Dictionary<int, Entity> _entities = new();
    private int _nextId = 1;

    public StatefulRepository()
    {
        _repository = Substitute.For<IRepository>();
        
        _repository.Save(Arg.Any<Entity>()).Returns(x =>
        {
            var entity = x.Arg<Entity>();
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            _entities[entity.Id] = entity;
            return Task.FromResult(entity);
        });
        
        _repository.GetById(Arg.Any<int>()).Returns(x =>
        {
            var id = x.Arg<int>();
            _entities.TryGetValue(id, out var entity);
            return Task.FromResult(entity);
        });
        
        _repository.Delete(Arg.Any<int>()).Returns(x =>
        {
            var id = x.Arg<int>();
            return Task.FromResult(_entities.Remove(id));
        });
    }
    
    public IRepository Repository => _repository;
    public IReadOnlyDictionary<int, Entity> StoredEntities => _entities;
}

// Usage in tests:
var statefulRepo = new StatefulRepository();
var service = new EntityService(statefulRepo.Repository);

var entity = new Entity { Name = "Test" };
await service.CreateEntity(entity);

// Entity now has an ID assigned
Assert.IsTrue(entity.Id > 0);
Assert.IsTrue(statefulRepo.StoredEntities.ContainsKey(entity.Id));
```

### Conditional return values

Implement complex conditional logic in return values:

```csharp
priceService.CalculatePrice(Arg.Any<Product>(), Arg.Any<Customer>()).Returns(x =>
{
    var product = x.Arg<Product>();
    var customer = x.Arg<Customer>();
    
    decimal basePrice = product.BasePrice;
    
    // Apply customer-specific discounts
    if (customer.IsPremium)
        basePrice *= 0.9m; // 10% discount
        
    // Apply product-specific logic
    if (product.Category == "Electronics" && customer.Age < 25)
        basePrice *= 0.95m; // Youth discount
        
    // Bulk discount
    if (product.Quantity > 100)
        basePrice *= 0.85m;
        
    return basePrice;
});
```

## Integration patterns

### Repository pattern with substitutes

Create sophisticated repository substitutes that behave like real databases:

```csharp
public static class RepositoryTestHelpers
{
    public static IUserRepository CreateInMemoryUserRepository()
    {
        var users = new List<User>();
        var repository = Substitute.For<IUserRepository>();
        
        repository.AddAsync(Arg.Any<User>()).Returns(x =>
        {
            var user = x.Arg<User>();
            user.Id = users.Count + 1;
            user.CreatedAt = DateTime.UtcNow;
            users.Add(user);
            return Task.FromResult(user);
        });
        
        repository.GetByIdAsync(Arg.Any<int>()).Returns(x =>
        {
            var id = x.Arg<int>();
            var user = users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        });
        
        repository.GetByEmailAsync(Arg.Any<string>()).Returns(x =>
        {
            var email = x.Arg<string>();
            var user = users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        });
        
        repository.UpdateAsync(Arg.Any<User>()).Returns(x =>
        {
            var updatedUser = x.Arg<User>();
            var existingUser = users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser != null)
            {
                existingUser.Name = updatedUser.Name;
                existingUser.Email = updatedUser.Email;
                existingUser.UpdatedAt = DateTime.UtcNow;
                return Task.FromResult(existingUser);
            }
            return Task.FromResult<User>(null);
        });
        
        return repository;
    }
}
```

### Service layer testing patterns

Test complex service interactions with multiple dependencies:

```csharp
[Test]
public async Task Should_process_order_with_complex_workflow()
{
    // Arrange
    var inventoryService = Substitute.For<IInventoryService>();
    var paymentService = Substitute.For<IPaymentService>();
    var emailService = Substitute.For<IEmailService>();
    var auditService = Substitute.For<IAuditService>();
    
    var orderService = new OrderService(
        inventoryService, paymentService, emailService, auditService);
    
    var order = new Order
    {
        Items = new[] { new OrderItem { ProductId = 1, Quantity = 2 } },
        Customer = new Customer { Email = "test@example.com" }
    };
    
    // Setup complex interaction chain
    inventoryService.CheckAvailabilityAsync(1, 2).Returns(true);
    inventoryService.ReserveAsync(1, 2).Returns(Task.FromResult("reservation-123"));
    
    paymentService.ProcessPaymentAsync(Arg.Any<decimal>(), Arg.Any<PaymentMethod>())
        .Returns(Task.FromResult(new PaymentResult { Success = true, TransactionId = "tx-456" }));
    
    emailService.SendOrderConfirmationAsync(Arg.Any<Order>())
        .Returns(Task.CompletedTask);
    
    auditService.LogOrderProcessedAsync(Arg.Any<Order>())
        .Returns(Task.CompletedTask);
    
    // Act
    var result = await orderService.ProcessOrderAsync(order);
    
    // Assert - Verify the entire workflow
    Assert.IsTrue(result.Success);
    
    // Verify inventory operations
    await inventoryService.Received(1).CheckAvailabilityAsync(1, 2);
    await inventoryService.Received(1).ReserveAsync(1, 2);
    
    // Verify payment processing
    await paymentService.Received(1).ProcessPaymentAsync(
        Arg.Is<decimal>(amount => amount > 0),
        Arg.Any<PaymentMethod>());
    
    // Verify notifications and auditing
    await emailService.Received(1).SendOrderConfirmationAsync(order);
    await auditService.Received(1).LogOrderProcessedAsync(
        Arg.Is<Order>(o => o.Status == OrderStatus.Completed));
}
```

## Error simulation patterns

### Simulating failures and retries

Test error handling and retry logic:

```csharp
[Test]
public async Task Should_retry_on_transient_failures()
{
    var httpClient = Substitute.For<IHttpClient>();
    var service = new ExternalApiService(httpClient);
    
    // Simulate failure then success pattern
    httpClient.GetAsync(Arg.Any<string>())
        .Returns(
            Task.FromException<HttpResponseMessage>(new HttpRequestException("Timeout")),
            Task.FromException<HttpResponseMessage>(new HttpRequestException("Network error")),
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"data\": \"success\" }")
            }));
    
    // Act
    var result = await service.GetDataWithRetryAsync("test-endpoint");
    
    // Assert
    Assert.IsNotNull(result);
    await httpClient.Received(3).GetAsync(Arg.Any<string>());
}
```

### Chaos testing patterns

Introduce random failures to test resilience:

```csharp
public class ChaosService<T> where T : class
{
    private readonly T _inner;
    private readonly Random _random = new();
    private readonly double _failureRate;

    public ChaosService(T inner, double failureRate = 0.1)
    {
        _inner = inner;
        _failureRate = failureRate;
    }

    public T CreateChaosProxy()
    {
        var substitute = Substitute.For<T>();
        
        // Intercept all calls and randomly fail
        substitute.Configure().Returns(x =>
        {
            if (_random.NextDouble() < _failureRate)
            {
                throw new InvalidOperationException("Chaos monkey struck!");
            }
            
            // Forward to real implementation
            return x.CallBase();
        });
        
        return substitute;
    }
}

// Usage:
var realService = new RealService();
var chaosService = new ChaosService<IService>(realService, failureRate: 0.2);
var service = chaosService.CreateChaosProxy();
```

## Performance testing patterns

### Load simulation

Simulate high-load scenarios:

```csharp
[Test]
public async Task Should_handle_concurrent_requests()
{
    var dataService = Substitute.For<IDataService>();
    var cache = Substitute.For<ICache>();
    var service = new CachedDataService(dataService, cache);
    
    // Setup responses with delays to simulate real-world latency
    dataService.GetDataAsync(Arg.Any<string>()).Returns(x =>
    {
        var key = x.Arg<string>();
        return Task.Delay(100).ContinueWith(_ => $"data-{key}");
    });
    
    cache.GetAsync(Arg.Any<string>()).Returns(Task.FromResult<string>(null));
    cache.SetAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);
    
    // Simulate concurrent load
    var tasks = Enumerable.Range(0, 100)
        .Select(i => service.GetDataAsync($"key-{i}"))
        .ToArray();
    
    var stopwatch = Stopwatch.StartNew();
    var results = await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    // Verify performance characteristics
    Assert.AreEqual(100, results.Length);
    Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000); // Should complete within 5 seconds
    
    // Verify caching behavior
    await dataService.Received(100).GetDataAsync(Arg.Any<string>());
    await cache.Received(100).SetAsync(Arg.Any<string>(), Arg.Any<string>());
}
```

## Testing patterns for specific scenarios

### Event-driven architectures

Test event publishers and subscribers:

```csharp
[Test]
public void Should_publish_events_in_correct_order()
{
    var eventBus = Substitute.For<IEventBus>();
    var service = new OrderProcessingService(eventBus);
    
    var publishedEvents = new List<IEvent>();
    
    eventBus.PublishAsync(Arg.Any<IEvent>()).Returns(x =>
    {
        publishedEvents.Add(x.Arg<IEvent>());
        return Task.CompletedTask;
    });
    
    // Act
    service.ProcessOrder(new Order { Id = 123 });
    
    // Assert event sequence
    Assert.AreEqual(3, publishedEvents.Count);
    Assert.IsInstanceOf<OrderStartedEvent>(publishedEvents[0]);
    Assert.IsInstanceOf<OrderProcessedEvent>(publishedEvents[1]);
    Assert.IsInstanceOf<OrderCompletedEvent>(publishedEvents[2]);
    
    // Verify event properties
    var startedEvent = (OrderStartedEvent)publishedEvents[0];
    Assert.AreEqual(123, startedEvent.OrderId);
}
```

### Workflow and state machines

Test complex workflows:

```csharp
[Test]
public void Should_execute_workflow_steps_in_order()
{
    var step1 = Substitute.For<IWorkflowStep>();
    var step2 = Substitute.For<IWorkflowStep>();
    var step3 = Substitute.For<IWorkflowStep>();
    
    var workflow = new Workflow(new[] { step1, step2, step3 });
    
    var executionOrder = new List<string>();
    
    step1.ExecuteAsync(Arg.Any<WorkflowContext>()).Returns(x =>
    {
        executionOrder.Add("Step1");
        return Task.FromResult(StepResult.Success);
    });
    
    step2.ExecuteAsync(Arg.Any<WorkflowContext>()).Returns(x =>
    {
        executionOrder.Add("Step2");
        return Task.FromResult(StepResult.Success);
    });
    
    step3.ExecuteAsync(Arg.Any<WorkflowContext>()).Returns(x =>
    {
        executionOrder.Add("Step3");
        return Task.FromResult(StepResult.Success);
    });
    
    // Act
    var result = await workflow.ExecuteAsync(new WorkflowContext());
    
    // Assert
    Assert.IsTrue(result.Success);
    CollectionAssert.AreEqual(new[] { "Step1", "Step2", "Step3" }, executionOrder);
}
```

## Best practices for advanced scenarios

### Substitute lifecycle management

```csharp
public class TestBase
{
    protected readonly Dictionary<Type, object> _substitutes = new();
    
    protected T GetSubstitute<T>() where T : class
    {
        if (!_substitutes.TryGetValue(typeof(T), out var substitute))
        {
            substitute = Substitute.For<T>();
            _substitutes[typeof(T)] = substitute;
        }
        return (T)substitute;
    }
    
    [TearDown]
    public void TearDown()
    {
        foreach (var substitute in _substitutes.Values)
        {
            if (substitute is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _substitutes.Clear();
    }
}
```

### Complex assertion patterns

```csharp
public static class AdvancedAssertions
{
    public static void ReceivedCallsInOrder<T>(T substitute, params Expression<Action<T>>[] calls) where T : class
    {
        var receivedCalls = substitute.ReceivedCalls().ToList();
        Assert.AreEqual(calls.Length, receivedCalls.Count, "Number of calls doesn't match");
        
        for (int i = 0; i < calls.Length; i++)
        {
            var expectedCall = calls[i];
            var actualCall = receivedCalls[i];
            
            // Extract method info from expression
            var methodCall = expectedCall.Body as MethodCallExpression;
            Assert.IsNotNull(methodCall, $"Call {i} is not a method call");
            
            Assert.AreEqual(methodCall.Method.Name, actualCall.GetMethodInfo().Name,
                $"Method name mismatch at position {i}");
        }
    }
}

// Usage:
AdvancedAssertions.ReceivedCallsInOrder(service,
    s => s.Initialize(),
    s => s.Process(Arg.Any<Data>()),
    s => s.Cleanup());
```

## Related topics

* [Callbacks](/help/callbacks/) - for complex interaction patterns
* [Threading](/help/threading/) - for concurrent scenarios
* [Argument matchers](/help/argument-matchers/) - for flexible matching
* [Troubleshooting](/help/troubleshooting/) - for common issues