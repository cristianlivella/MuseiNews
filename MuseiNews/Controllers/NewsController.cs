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
    [Route("news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsApiContext _context;

        public NewsController(NewsApiContext context)
        {
            _context = context;
        }

        // GET: news
        [HttpGet]
        public dynamic GetNews()
        {
            return _context.News.Select(n => new { n.Id, n.Title, n.Description, n.Url, n.Timestamp, n.Picture }).OrderBy(n => n.Timestamp).ToArray();
        }

        // GET: news/{id}
        [HttpGet("{id}")]
        public dynamic GetNews(int id)
        {
            var news = _context.News.Find(id);

            if (news == null)
            {
                return NotFound();
            }

            return new { news.Id, news.Title, news.Description, news.Url, news.Timestamp, news.Picture };
        }
    }
}
