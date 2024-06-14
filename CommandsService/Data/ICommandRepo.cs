using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepo
{
    bool SaveChanges();

    // Platform
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
    bool ExternalPlatformExists(int externalPlatformId);


    // Command
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId ,int commandId);
    void createCommand(int platformId , Command command);
}