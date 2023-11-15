using FormulaOneApp.Data;
using FormulaOneApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneApp.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")] // api/teams
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbcontext _context;

        public TeamsController(AppDbcontext context)
        {
            _context = context;
        }
        //private static List<Team> teams = new List<Team>()
        //{
        //    new Team()
        //    {
        //        Id = 1,
        //        Country = "Germany",
        //        TeamName = "Mercedes",
        //        TeamPrinciple = "adolf hitler"
        //    },
        //    new Team()
        //    {
        //        Id = 2,
        //        Country = "Italy",
        //        TeamName = "Ferrari",
        //        TeamPrinciple = "Mussolini"
        //    },
        //    new Team()
        //    {
        //        Id = 3,
        //        Country = "Switzerland",
        //        TeamName = "Alpha romeo",
        //        TeamPrinciple = "mother dairy"
        //    }
        //};

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var teams = await _context.Teams.ToListAsync();
            return Ok(teams);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == id);

            if(team == null)
            {
                return BadRequest("Invalid id");
            }

            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", team.Id, team);
        }

        [HttpPatch]
        public async Task<IActionResult> Update(int id, string country)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                return BadRequest("Invalid id");
            }

            team.Country = country;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                return BadRequest("Invalid id");
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
