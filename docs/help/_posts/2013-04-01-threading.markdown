---
title: Threading
layout: post
---

It is fairly standard for production code to call a substitute from multiple threads, but we should avoid having our test code configure or assert on a substitute while it is also be used from other threads in production code. 

Although this particular issue has been mitigated by work in [#452]({{ site.repo }}/pull/462), issue [#256]({{ site.repo }}/issues/256) shows the types of problems that can occur if we're not careful with threading.

To avoid this sort of problem, make sure your test has finished configuring its substitutes before exercising the production code, then make sure the production code has completed before your test asserts on `Received()` calls.