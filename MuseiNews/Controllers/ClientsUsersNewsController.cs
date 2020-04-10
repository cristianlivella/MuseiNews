using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseiNews.DAL;
using MuseiNews.Filters;
using MuseiNews.Models;

namespace MuseiNews.Controllers
{
    [Route("clients/{clientId}/users/{internalUserId}/news")]
    [ServiceFilter(typeof(UserNewsFilter))]
    [ApiController]
    public class ClientsUsersNewsController : ControllerBase
    {
        private readonly NewsApiContext _context;

        public ClientsUsersNewsController(NewsApiContext context)
        {
            _context = context;
        }

        // GET: clients/{clientId}/users/{internalUserId}/news
        [HttpGet]
        public ActionResult<IEnumerable<News>> GetNews(int userId, int clientId, int internalUserId)
        {
            return GetNewsList(userId).ToList();
        }

        // GET: clients/{clientId}/users/{internalUserId}/news/notRead
        [HttpGet("notRead")]
        public ActionResult<IEnumerable<News>> GetNewsNotRead(int userId, int clientId, int internalUserId)
        {
            var news = GetNewsList(userId);
            return news.Where(n => n.Read == false).ToList();
        }

        // GET: clients/{clientId}/users/{internalUserId}/news/{id}
        [HttpGet("{id}")]
        public ActionResult<News> GetSingleNews(int userId, int clientId, int internalUserId, int id)
        {
            var news = _context.News.Find(id);
            if (news == null)
            {
                return NotFound();
            }
            var userReadIt = _context.NewsReads.Where(n => n.NewsId==news.Id && n.UserId == userId).Count();
            news.Read = (userReadIt > 0);
            return news;
        }

        // PUT: clients/{clientId}/users/{internalUserId}/news/{id}
        [HttpPut("{id}")]
        public IActionResult PutNews(int id, int userId, int clientId, int internalUserId, News news)
        {
            if (id != news.Id)
            {
                return BadRequest();
            }
            if (!_context.News.Any(n => n.Id == id))
            {
                return NotFound();
            }
            var userReadIt = (_context.NewsReads.Where(n => n.NewsId == news.Id && n.UserId == userId).Count() > 0);
            if (!userReadIt && news.Read)
            {
                var newsRead = new NewsRead();
                newsRead.NewsId = news.Id;
                newsRead.UserId = userId;
                _context.NewsReads.Add(newsRead);
                _context.SaveChanges();
            }
            if (userReadIt && !news.Read)
            {
                return Conflict();
            }
            return NoContent();
        }

        public IEnumerable<News> GetNewsList(int userId)
        {
            var newReadUser = _context.NewsReads.Where(n => n.UserId == userId);
            var news = _context.News.OrderBy(n => n.Timestamp).ToList();
            for (int i = 0; i < news.Count; i++)
            {
                news[i].Read = (newReadUser.Where(n => n.NewsId == news[i].Id).ToList().Count > 0);
            }
            return news;
        }
    }
}
