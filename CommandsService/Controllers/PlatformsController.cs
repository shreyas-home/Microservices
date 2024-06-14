using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo repo;
    private readonly IMapper mapper;

    public PlatformsController(
            ICommandRepo repo,
            IMapper mapper
    )
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        Console.WriteLine("--> Getting all platforms from commands service");

        var platforms = repo.GetAllPlatforms();

        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpPost]
    public ActionResult TestInBoundConnection(){
        Console.WriteLine("--> Inbound Post from commands service platform controller");

        return Ok("--> Inbound Post from commands service platform controller");
    }
}
