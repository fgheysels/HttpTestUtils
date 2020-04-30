using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HttpTestUtils
{
    public static class HttpClientMock
    {
        public static HttpClient SetupHttpClientWithJsonResponse<T>(HttpStatusCode responseStatusCode, T responseBody)
        {
            var messageHandler = 
                new TestHttpMessageHandler( _ => Task.FromResult(new HttpResponseMessage(responseStatusCode) { Content = new StringContent(JsonConvert.SerializeObject(responseBody), Encoding.UTF8, "application/json") }));

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
