using RestSharp;
using System;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://contactbook.velinski.repl.co/api";

        [SetUp]

        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]

        public void Test_GetContacts_ByName()
        {
           var request = new RestRequest("contacts", Method.Get);

            var response = this.client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(contacts[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Jobs"));
        }

        [Test]

        public void Test_GetContacts_ByKeyword_ValidResult()
        {
            var request = new RestRequest("contacts/search/albert", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(contacts[0].firstName, Is.EqualTo("Albert"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Einstein"));
        }

        [Test]

        public void Test_GetContacts_ByKeyword_InvalidResult()
        {
            var request = new RestRequest("contacts/search/missing" + DateTime.Now.Ticks, Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("[]"));
        }


        [Test]

        public void Test_CreateContact_InvalidData()
        {
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend",
            };

            request.AddBody(reqBody);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));
        }

        [Test]

        public void Test_CreateContact_ValidData()
        {
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                firstName = "Elton",
                lastName = "John",
                email = "elton@gmail.com",
                phone = "+1 900 600 300",
                comments = "Singer",
            };

            request.AddBody(reqBody);

            var response = this.client.Execute(request);

            var contactObject = JsonSerializer.Deserialize<contactObject>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            Assert.That(contactObject.msg, Is.EqualTo("Contact added."));
            Assert.That(contactObject.contact.firstName, Is.EqualTo("Elton"));
            Assert.That(contactObject.contact.lastName, Is.EqualTo("John"));
            Assert.That(contactObject.contact.email, Is.EqualTo("elton@gmail.com"));
            Assert.That(contactObject.contact.phone, Is.Not.Empty);
            Assert.That(contactObject.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactObject.contact.comments, Is.EqualTo("Singer"));
        }

        [Test]

        public void Test_GetContactById()
        {
            var request = new RestRequest("contacts", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contacts>>(response.Content);

            Assert.That(contacts[0].id, Is.EqualTo(1));
        
        }

       
    }


}