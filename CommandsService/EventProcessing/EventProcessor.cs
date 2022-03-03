using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMsg)
        {
            Console.WriteLine("==> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMsg);

            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("==> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("==> Platform Published Event Undetermined");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMsg)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMsg);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("==> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("==> Platform already exists...");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"==> Could not add Platform to DB {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}