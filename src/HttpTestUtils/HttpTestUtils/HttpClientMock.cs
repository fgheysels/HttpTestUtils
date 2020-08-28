using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpTestUtils
{
    public static class HttpClientMock
    {
        public static HttpClient SetupHttpClientWithJsonResponse<T>(HttpStatusCode responseStatusCode, T responseBody)
        {
            return SetupHttpClientWithJsonResponse(new HttpResponseContent<T>(responseStatusCode, responseBody));
        }

        public static HttpClient SetupHttpClientWithJsonResponse<T>(HttpResponseContent<T> response)
        {
            var messageHandler =
                new TestHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(response.StatusCode) { Content = new StringContent(JsonConvert.SerializeObject(response.Content), Encoding.UTF8, "application/json") }));

            return new HttpClient(messageHandler);
        }

        /// <summary>
        /// Sets up a HttpClientMock that will return another response on each invocation.
        /// </summary>
        /// <remarks>The HttpClient will return the responses in the same order as they appear in the <paramref name="responses"/> parameter.</remarks>
        /// <returns>A HttpClient instance that can be used for test purposes.</returns>
        public static HttpClient SetupHttpClientWithMultipleJsonResponses<T>(IEnumerable<HttpResponseContent<T>> responses)
        {
            var responseQueue = new Queue<HttpResponseContent<T>>();

            foreach (var response in responses)
            {
                responseQueue.Enqueue(response);
            }

            var messageHandler =
                new TestHttpMessageHandler(_ =>
                {
                    var responseContent = responseQueue.Dequeue();

                    var response = new HttpResponseMessage(responseContent.StatusCode)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(responseContent.Content),
                                                    Encoding.UTF8,
                                                    "application/json")
                    };

                    return Task.FromResult(response);
                });

            return new HttpClient(messageHandler);
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _createResponseFunction;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestHttpMessageHandler"/> class.
            /// </summary>
            public TestHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> createResponse)
            {
                _createResponseFunction = createResponse;
            }

            /// <summary>Send an HTTP request as an asynchronous operation.</summary>
            /// <param name="request">The HTTP request message to send.</param>
            /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
            /// <returns>The task object representing the asynchronous operation.</returns>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="request">request</paramref> was null.</exception>
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await _createResponseFunction(request);
            }
        }
    }
}
