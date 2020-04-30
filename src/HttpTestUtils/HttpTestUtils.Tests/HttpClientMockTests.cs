using System;
using System.Net;
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

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
