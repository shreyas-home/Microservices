using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMapper mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        this.scopeFactory = scopeFactory;
        this.mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished :
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event...");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform published Event ");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Undetermined Event");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using(var scope = scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = mapper.Map<Platform>(platformPublishedDto);

                if(!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                    Console.WriteLine("--> Platform Added");
                }
                else
                {
                    Console.WriteLine("--> External Platform already exists");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could Not add Platform To DB {ex.Message}");
            }
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}