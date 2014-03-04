using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Testing;
using Nancy.Authentication.Forms;
using Nancy.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FreelanceManager.Web
{
    public class Test
    {
        [Fact]
        public void Test_2()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "Test client"
                });
            });

            var response = browser.Get("/read/clients").Body.AsString();
        }

        [Fact]
        public void Test_3()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "Test client"
                });
            });

            var response = browser.Get("/read/clients").Body.AsString();
        }

        [Fact]
        public void Test_4()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "Test client"
                });
            });

            dynamic response = JArray.Parse( browser.Get("/read/clients").Body.AsString());

            Assert.Equal("Test client", (string)response[0].Name);
        }

        [Fact]
        public void Test_5()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "Test client"
                });
            });

            var response = browser.Get("/read/clients").Body.AsString();
        }
    }
}
