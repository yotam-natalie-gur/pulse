﻿using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Core;
using Pulse.Core.Calls;

var services = new ServiceCollection();
const string serverHttpClient = "Pulse.Server";
services.AddHttpClient(serverHttpClient, client =>
{
    client.BaseAddress = new Uri("http://ec2-3-65-21-97.eu-central-1.compute.amazonaws.com:5000");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "{MY_TOKEN}");
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
    await file.CopyToAsync(stream);
}
else
{
    Console.WriteLine("Polling...");
    _ = serviceProvider.GetRequiredService<IncomingCallPoller>();
}


Console.WriteLine("Done");

await Task.Delay(-1);