using System.Net;

namespace HttpTestUtils
{
    public class HttpResponseContent<TContent>
    {
        public HttpStatusCode StatusCode { get; }
        public TContent Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseContent"/> class.
        /// </summary>
        public HttpResponseContent(HttpStatusCode statusCode, TContent content)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}