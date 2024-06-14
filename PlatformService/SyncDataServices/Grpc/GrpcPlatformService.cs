using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepo repo;
    private readonly IMapper mapper;

    public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();
        var platforms = repo.GetAllPlatforms();

        foreach(var platform in platforms)
        {
            response.Platform.Add(mapper.Map<GrpcPlatformModel>(platform));
        }
        return Task.FromResult(response);
    }

}