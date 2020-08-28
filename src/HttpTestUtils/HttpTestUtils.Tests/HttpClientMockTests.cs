using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace HttpTestUtils.Tests
{
    public class HttpClientMockTests
    {
        [Fact]
        public async Task HttpClientMockReturnsExpectedResult()
        {
            var sut = HttpClientMock.SetupHttpClientWithJsonResponse(HttpStatusCode.OK, new Person { Id = 1, Name = "Frederik" });

            var response = await sut.GetAsync(new Uri("http://localhost"));

            var returnedPerson = JsonConvert.DeserializeObject<Person>(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, returnedPerson.Id);
            Assert.Equal("Frederik", returnedPerson.Name);
        }

        [Fact]
        public async Task HttpClientMockReturnsMultipleResults()
        {
            var responses = new[]
            {
                new HttpResponseContent<Person>(HttpStatusCode.OK, new Person(1, "Frederik")),
                new HttpResponseContent<Person>(HttpStatusCode.OK, new Person(2, "Barbara")),
                new HttpResponseContent<Person>(HttpStatusCode.Accepted, new Person(3, "Bob"))
            };

            var sut = HttpClientMock.SetupHttpClientWithMultipleJsonResponses(responses);

            var response1 = await sut.GetAsync(new Uri("http://localhost"));
            var response2 = await sut.GetAsync(new Uri("http://localhost"));
            var response3 = await sut.GetAsync(new Uri("http://localhost"));

            var result1 = GetContentFromResponseAsync<Person>(response1);
            var result2 = GetContentFromResponseAsync<Person>(response2);
            var result3 = GetContentFromResponseAsync<Person>(response3);

            Assert.Equal(1, result1.Id);
            Assert.Equal(2, result2.Id);
            Assert.Equal(3, result3.Id);
        }

        [Fact]
        public async Task HttpClientMockWithMultipleResponses_ThrowsExceptionWhenOutOfResponses()
        {
            var responses = new[]
            {
                new HttpResponseContent<Person>(HttpStatusCode.OK, new Person(1, "Frederik")),
                new HttpResponseContent<Person>(HttpStatusCode.OK, new Person(2, "Barbara")),
                new HttpResponseContent<Person>(HttpStatusCode.Accepted, new Person(3, "Bob"))
            };

            var sut = HttpClientMock.SetupHttpClientWithMultipleJsonResponses(responses);

            for (int i = 0; i < 3; i++)
            {
                await sut.GetAsync(new Uri("http://localhost"));
            }

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetAsync(new Uri("http://localhost")));
        }

        private static async Task<T> GetContentFromResponseAsync<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public Person() { }

            public Person(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}
