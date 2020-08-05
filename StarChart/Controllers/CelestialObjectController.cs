using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects
                .Where(x => x.OrbitedObjectId == celestialObject.Id)
                .ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(x => x.Name == name)
                .ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestial in celestialObjects)
            {
                celestial.Satellites = _context.CelestialObjects
                .Where(x => x.OrbitedObjectId == celestial.Id)
                .ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestial in celestialObjects)
            {
                celestial.Satellites = _context.CelestialObjects
                .Where(x => x.OrbitedObjectId == celestial.Id)
                .ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id },
                celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestial = _context.CelestialObjects
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = celestialObject.Name;
            celestial.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestial.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = name;

            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(x => x.Id == id || x.OrbitedObjectId == id)
                .ToList();

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects
                .RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
