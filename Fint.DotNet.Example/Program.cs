using System;
using System.Linq;
using System.Net.Http;
using HalClient.Net;
using HalClient.Net.Parser;
using IdentityModel.Client;

namespace Fint.DotNet.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            var tokenClient = new TokenClient(
                "https://namidp01.rogfk.no/nidp/oauth/nam/token",
                "6e1cf7b4-b107-42b3-9435-8fda70726c6a",
                "6y4FUuP9BfAXeVqguNKT0ofToIwN5RdB1PaUvx_nCMiQbH9NeGq3pp0jQB9zOQ0APOxEbodzJXp-8RVux6318A"
            );
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("pwfatut", "pwfatut", "fint-client")
                .Result;

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(tokenResponse.AccessToken);

                var response = client
                    .GetAsync(new Uri(
                        "https://beta.felleskomponent.no/administrasjon/personal/person")).Result;
                var links = response.Resource.Links;
                var embedded = response.Resource.Embedded;

                var firstObject = embedded["_entries"].First().State;
                var selfLink = links["self"].Single().Href;
                var person = PersonFactory.create(firstObject);
                
                Console.WriteLine("Self link: " + selfLink);
                Console.WriteLine("Person: " + person.Navn.Fornavn + " " + person.Navn.Etternavn);
            }
        }
    }
}