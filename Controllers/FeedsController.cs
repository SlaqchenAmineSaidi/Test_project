using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test_project.Data;
using Test_project.Models;

namespace Test_project.Controllers
{
    public class FeedsController : Controller
    {
        private readonly Test_projectContext _context;

        public FeedsController(Test_projectContext context)
        {
            _context = context;
        }

        // GET: Feeds
        public async Task<IActionResult> Index(string search)
        {
            IQueryable<Feed> queryableFeeds = _context.Feed.Include(f => f.Articles);

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryableFeeds = queryableFeeds.Where(feed => feed.Name.Contains(search));
                // You can also add filtering by article title
                // queryableFeeds = queryableFeeds.Where(feed => feed.Articles.Any(article => article.Title.Contains(search)));
            }

            var feeds = await queryableFeeds.ToListAsync();
            return View(feeds);
        }


        // GET: Feeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feed = await _context.Feed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feed == null)
            {
                return NotFound();
            }

            return View(feed);
        }

        // GET: Feeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Feeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feed);
        }

        // GET: Feeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feed = await _context.Feed.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }
            return View(feed);
        }

        // POST: Feeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url")] Feed feed)
        {
            if (id != feed.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedExists(feed.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feed);
        }

        // GET: Feeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feed = await _context.Feed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feed == null)
            {
                return NotFound();
            }

            return View(feed);
        }

        // POST: Feeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feed = await _context.Feed.FindAsync(id);
            _context.Feed.Remove(feed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedExists(int id)
        {
            return _context.Feed.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> FilterArticlesByDate(DateTime fromDate, DateTime toDate)
        {
            var filteredFeeds = await _context.Feed
                .Where(feed => feed.Articles.Any(article => article.PublishDate >= fromDate && article.PublishDate <= toDate))
                .Include(feed => feed.Articles)
                .ToListAsync();

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View("Index", filteredFeeds);
        }



        public async Task<IActionResult> ReloadArticles()
        {
            var feeds = await _context.Feed.ToListAsync();

            foreach (var feed in feeds)
            {
                try
                {
                    // Implement RSS parsing to reload articles for the feed
                    List<Article> newArticles = ParseRss(feed.Url);

                    // Remove existing articles and add the new ones
                    feed.Articles.Clear();
                    foreach (var article in newArticles)
                    {
                        feed.Articles.Add(article);
                    }
                }
                catch (Exception ex)
                {
                   
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        private List<Article> ParseRss(string feedUrl)
        {
            List<Article> newArticles = new List<Article>();

            // Implement your RSS parsing logic here and populate newArticles list
            // Example using a library like System.ServiceModel.Syndication:
            var reader = XmlReader.Create(feedUrl);
            var feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items)
            {
                var article = new Article
                {
                    Title = item.Title.Text,
                    PublishDate = item.PublishDate.LocalDateTime,
                    // Set other properties as needed
                };

                newArticles.Add(article);
            }

            return newArticles;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int[] selectedFeeds)
        {
            if (selectedFeeds != null && selectedFeeds.Length > 0)
            {
                foreach (var feedId in selectedFeeds)
                {
                    var feed = await _context.Feed.FindAsync(feedId);
                    if (feed != null)
                    {
                        _context.Feed.Remove(feed);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }

}
