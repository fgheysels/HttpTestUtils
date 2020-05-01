# Introduction

`HttpTestUtils` contains some classes which make it easier to mock out dependencies on `HttpClient` during unit-tests.

## HttpClientMock

When code has a dependency on `HttpClient` it might be hard to test that code since in a unit-test you clearly do not want to send real HTTP messages.  The `HttpClientMock` allows you to predefine the behaviour that must be executed by the `HttpClient` class.
Have a look at the `HttpClientMockTests` in the HttpTestUtils.Tests project to find out how to make use of it.
