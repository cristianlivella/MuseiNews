using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MuseiNews.DAL;
using MuseiNews.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MuseiNews.Tasks
{
    /*
     * Reference: https://codeburst.io/schedule-cron-jobs-using-hostedservice-in-asp-net-core-e17c47ba06
    */
    public class UpdateNews : IHostedService, IDisposable
    {
        private System.Timers.Timer _timer;
        private WebClient client = new WebClient();
        private readonly CronExpression _expression = CronExpression.Parse("*/5 * * * *");
        private readonly TimeZoneInfo _timeZoneInfo = TimeZoneInfo.Local;

        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateNews(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                XmlReader reader = XmlReader.Create(new StreamReader(client.OpenRead("https://www.intopic.it/rss/arte/musei/"), Encoding.GetEncoding("ISO-8859-1")), settings);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NewsApiContext>();
                    var items = feed.Items.OrderBy(i => i.PublishDate.ToUnixTimeSeconds());
                    foreach (SyndicationItem item in items)
                    {
                        var url = item.Links[0].Uri.ToString();
                        if (!context.News.Any(n => n.Url == url))
                        {
                            News news = new News();
                            news.Title = item.Title.Text;
                            var splittedDescription = item.Summary.Text.Split("<br />", 2);
                            news.Description = splittedDescription[0].Trim();
                            news.Url = url;
                            news.Timestamp = item.PublishDate.ToUnixTimeSeconds();
                            if (item.Links.Count > 1)
                            {
                                news.Picture = item.Links[1].Uri.ToString();
                            }
                            else
                            {
                                news.Picture = "";
                            }
                            context.News.Add(news);
                            context.SaveChanges();
                        }
                    }
                }
                    
            }
            catch { }
            await Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }

    }


}
