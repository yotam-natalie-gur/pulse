﻿using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Core;
using Pulse.Core.Calls;

var services = new ServiceCollection();
const string serverHttpClient = "Pulse.Server";
services.AddHttpClient(serverHttpClient, client =>
{
    client.BaseAddress = new Uri("https://pulse.gurgaller.com");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ5b3RhbSIsIm5iZiI6MTY2MzkzNTY3OSwiZXhwIjoxNjY0NTQwNDc5LCJpYXQiOjE2NjM5MzU2Nzl9.eIrF51xJl0hh817vTJOjY7Olrpp8mwTMhUsDLj-lhDM");
});
services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(serverHttpClient));
services.AddPulse();

var serviceProvider = services.BuildServiceProvider();

Console.Write("Initiator or Receiver? i/r");

var answer = Console.ReadLine();

if (answer == "i")
{
    Console.Write("Who do you want to call? ");
    var callee = Console.ReadLine();
    Console.WriteLine("Calling...");
    var callInitiator = serviceProvider.GetRequiredService<ICallInitiator>();
    var stream = await callInitiator.CallAsync(callee!);

    await using var file = File.OpenRead("music.wav");
    await Task.Delay(75);  // TODO on a lower level - Let's wait a bit for the other party to be ready
    var sw = Stopwatch.StartNew();
    await file.CopyToAsync(stream, bufferSize: 320);  // TODO: put this on a lower level
    sw.Stop();
    Console.WriteLine($"Sent {file.Length} bytes in {sw.Elapsed.TotalSeconds} seconds");
}
else
{
    Console.WriteLine("Polling...");
    _ = serviceProvider.GetRequiredService<IncomingCallPoller>();
}


Console.WriteLine("Done");

await Task.Delay(-1);