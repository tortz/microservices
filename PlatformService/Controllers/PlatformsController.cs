using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Model;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient
            )
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        // GET: api/Platforms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
        {
            var platforms = await _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        // GET: api/Platforms/5
        [HttpGet("{id}", Name="GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            var platform = await _repo.GetPlaformById(id);

            if (platform == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map< PlatformReadDto>(platform));
        }

        /*// PUT: api/Platforms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlatform(int id, Platform platform)
        {
            if (id != platform.Id)
            {
                return BadRequest();
            }

            _context.Entry(platform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/Platforms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformDto)
        {
            var platform = _mapper.Map<Platform>(platformDto);
            _repo.CreatePlatform(platform);
            await _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not POST synchronously: {ex.Message}");
            }

            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            try
            {
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronous message: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetPlatformById), 
                new { id = platform.Id },
                platformReadDto);
        }

        /*// DELETE: api/Platforms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var platform = await _context.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }

            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

       /* private bool PlatformExists(int id)
        {
            return _context.Platforms.Any(e => e.Id == id);
        }*/
    }
}
