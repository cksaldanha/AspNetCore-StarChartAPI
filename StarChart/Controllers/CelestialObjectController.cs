using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;
using System.Collections.Generic;
using System.Linq;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            this._context = context;
        }


        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestial = _context.CelestialObjects.SingleOrDefault(c => c.Id == id);
            if (celestial == null) return NotFound();

            var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            celestial.Satellites = satellites;

            return Ok(celestial);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name)?.ToList();

            if (celestialObjects == null || celestialObjects.Count == 0) return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject model)
        {
            var celestialObject = new CelestialObject
            {
                Id = model.Id,
                Name = model.Name,
                OrbitalPeriod = model.OrbitalPeriod,
                OrbitedObjectId = model.OrbitedObjectId,
                Satellites = model.Satellites
            };

            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject model)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(c => c.Id == id);
            if (celestialObject == null) return NotFound();

            celestialObject.Name = model.Name;
            celestialObject.OrbitalPeriod = model.OrbitalPeriod;
            celestialObject.OrbitedObjectId = model.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(c => c.Id == id);
            if (celestialObject == null) return NotFound();

            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();

            if (celestialObjects.Count == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
