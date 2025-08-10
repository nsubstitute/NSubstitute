---
title: Threading
---

NSubstitute is designed to work safely in multi-threaded environments, but there are important guidelines to follow when using substitutes with multiple threads.

## Basic thread safety

It is fairly standard for production code to call a substitute from multiple threads. NSubstitute substitutes are thread-safe for:

* **Concurrent calls**: Multiple threads can safely call methods on the same substitute simultaneously
* **Reading configured behavior**: Calling methods that have been configured with `.Returns()` is thread-safe
* **Recording calls**: NSubstitute safely records all calls made from any thread

## Avoiding race conditions in tests

While substitutes themselves are thread-safe, we should avoid having our test code configure or assert on a substitute while it is also being used from other threads in production code.

Although this particular issue has been mitigated by work in [#452]({{ site.repo }}/pull/462), issue [#256]({{ site.repo }}/issues/256) shows the types of problems that can occur if we're not careful with threading.

To avoid this sort of problem, make sure your test has finished configuring its substitutes before exercising the production code, then make sure the production code has completed before your test asserts on `Received()` calls.

## Best practices

### 1. Configure before, assert after

```csharp
[Test]
public void Should_handle_concurrent_calls_safely()
{
    // Arrange: Configure substitute before starting concurrent operations
    var logger = Substitute.For<ILogger>();
    var service = new DataProcessor(logger);
    
    // Act: Start concurrent operations
    var tasks = Enumerable.Range(0, 10)
        .Select(i => Task.Run(() => service.ProcessData($"data-{i}")))
        .ToArray();
    
    // Wait for all operations to complete
    Task.WaitAll(tasks);
    
    // Assert: Now it's safe to check what happened
    logger.Received(10).LogInfo(Arg.Any<string>());
}
```

### 2. Use synchronization when needed

If your test needs to coordinate between threads, use proper synchronization:

```csharp
[Test]
public void Should_log_from_background_thread()
{
    var logger = Substitute.For<ILogger>();
    var resetEvent = new ManualResetEventSlim(false);
    
    // Configure the substitute
    logger.LogInfo(Arg.Any<string>()).Returns(x =>
    {
        resetEvent.Set(); // Signal that the call was made
        return Task.CompletedTask;
    });
    
    // Start background work
    Task.Run(() => logger.LogInfo("Background message"));
    
    // Wait for the call to complete
    Assert.IsTrue(resetEvent.Wait(TimeSpan.FromSeconds(5)));
    
    // Now assert
    logger.Received(1).LogInfo("Background message");
}
```

### 3. Avoid configuring during execution

❌ **Don't do this** (race condition):

```csharp
// BAD: Configuring while other threads might be calling
Task.Run(() => repository.Save(data));
repository.Save(Arg.Any<Data>()).Returns(true); // Race condition!
```

✅ **Do this instead**:

```csharp
// GOOD: Configure first, then execute
repository.Save(Arg.Any<Data>()).Returns(true);
var task = Task.Run(() => repository.Save(data));
task.Wait();
```

## Thread-safe configuration patterns

### Using callbacks for thread coordination

```csharp
[Test]
public void Should_coordinate_across_threads()
{
    var service = Substitute.For<IDataService>();
    var callOrder = new List<string>();
    var lockObject = new object();
    
    service.ProcessItem(Arg.Any<string>()).Returns(x =>
    {
        lock (lockObject)
        {
            callOrder.Add(x.Arg<string>());
        }
        return "processed";
    });
    
    // Execute concurrent operations
    var tasks = new[]
    {
        Task.Run(() => service.ProcessItem("item1")),
        Task.Run(() => service.ProcessItem("item2")),
        Task.Run(() => service.ProcessItem("item3"))
    };
    
    Task.WaitAll(tasks);
    
    // Verify all items were processed (order may vary)
    lock (lockObject)
    {
        Assert.AreEqual(3, callOrder.Count);
        CollectionAssert.Contains(callOrder, "item1");
        CollectionAssert.Contains(callOrder, "item2");
        CollectionAssert.Contains(callOrder, "item3");
    }
}
```

## Troubleshooting threading issues

If you encounter unexpected behavior in multi-threaded scenarios:

1. **Check configuration timing**: Ensure all `.Returns()`, `.Throws()`, and other configurations are set before concurrent execution
2. **Verify assertion timing**: Make sure all background operations complete before calling `.Received()`
3. **Use timeouts**: When waiting for async operations, always use timeouts to avoid hanging tests
4. **Consider using `ConfigureAwait(false)`**: In library code, this can help avoid deadlocks

## Related topics

* [Callbacks](/help/callbacks/) - for coordinating behavior across threads
* [Auto and recursive mocks](/help/auto-and-recursive-mocks/) - thread-safe by default
* [Return values](/help/return-for-args/) - setting up thread-safe return behavior