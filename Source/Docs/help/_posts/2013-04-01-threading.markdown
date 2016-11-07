---
title: Threading
layout: post
---

It is fairly standard for production code to call a substitute from multiple threads, but we should avoid having our test code configure or assert on a substitute while it is also be used from other threads in production code. See [Issue #256](https://github.com/nsubstitute/NSubstitute/issues/256) for an example of how doing this can result in problems.

To avoid this sort of problem, make sure your test has finished configuring its substitutes before exercising the production code, then make sure the production code has completed before your test asserts on `Received()` calls.

