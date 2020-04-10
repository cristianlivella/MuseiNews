using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseiNews.DAL;
using MuseiNews.Models;

namespace MuseiNews.Controllers
{
    [Route("clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly NewsApiContext _context;

        public ClientsController(NewsApiContext context)
        {
            _context = context;
        }

        // POST: clients
        [HttpPost]
        public ActionResult<Client> PostClient()
        {
            Client client = new Client();
            client.SecretToken = RandomString(64);
            _context.Clients.Add(client);
            _context.SaveChanges();
            Response.StatusCode = 201;
            return client;
        }

        public static string RandomString(int length)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new String(stringChars);
        }

    }
}
