using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo repo;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IMessageBusClient messageBusClient;

    public PlatformsController(
        IPlatformRepo repo , 
        IMapper mapper ,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        this.repo = repo;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        this.messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms...");

        var platforms = repo.GetAllPlatforms();

        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        
    }

    [HttpGet("{id}",Name ="GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = repo.GetPlatformById(id);

        if(platform is not null)
        {
            return Ok(mapper.Map<PlatformReadDto>(platform));
        }

        return NotFound();

    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto createDto)
    {
        var platformModel = mapper.Map<Platform>(createDto);

        repo.CreatePlatform(platformModel);
        repo.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);
        
        
        // Send Message Synchronously HTTP
        try
        {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }

        // Send Message Asynchronously RabbitMQ
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }


        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id} , platformReadDto);
    }
}