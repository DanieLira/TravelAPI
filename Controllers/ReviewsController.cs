using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using TravelAPI.DataAccess;
using TravelAPI.Models;

namespace TravelAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly TravelContext _context;

        public ReviewsController(TravelContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews(string? country = null, string? city = null, int authorId = 0, int rating = -1)
        {
            var query = _context.Reviews.AsQueryable();

            if (country != null)
            {
                query = query.Where(entry => entry.Country == country);
            }

            if (city != null)
            {
                query = query.Where(entry => entry.City == city);
            }

            if (authorId > 0)
            {
                query = query.Where(entry => entry.AuthorId == authorId);
            }

            if (rating > -1)
            {
                query = query.Where(entry => entry.Rating >= rating);
            }

            return await _context.Reviews.ToListAsync();
        }

        [HttpGet("mostReviewed")]
        public async Task<ActionResult<IEnumerable<Review>>> GetByMostReviews()
        {
            var query = _context.Reviews.AsQueryable();

            List<string> cities = GetDistinctCities(query);

            string? mostPopularCity = cities.GroupBy(e => e)
                                            .OrderByDescending(e => e.Count())
                                            .Select(e => e.Key)
                                            .FirstOrDefault();

            query = query.Where(entry => entry.City == mostPopularCity);

            return await query.ToListAsync();
        }

        [HttpGet("random")]
        public async Task<ActionResult<IEnumerable<Review>>> GetRandom()
        {
            var query = _context.Reviews.AsQueryable();

            List<string> cities = GetDistinctCities(query);
            
            Random rnd = new Random();

            var randomCity = cities[rnd.Next(cities.Count)];

            query = query.Where(entry => entry.City == randomCity);

            return await query.ToListAsync();
        }


        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.ReviewId)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            var user = await _context.Users.FindAsync(review.AuthorId);

            if(user == null)
            {
                return BadRequest();
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id, int authorId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.ReviewId != authorId)
            {
                return BadRequest();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }

        private static List<string> GetDistinctCities(IQueryable<Review> query)
        {
            return query.Select(e => e.City).Distinct().ToList();
        }
    }
}
