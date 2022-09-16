﻿using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Pulse.Server.Core;

public class InMemoryCallMatcher
{
    private readonly ConcurrentDictionary<string, ConnectionDetails> pendingCalls = new();
    private readonly ConcurrentDictionary<string, ConnectionDetails> acceptedCalls = new();

    public async Task<ConnectionDetails> InitiateCallAsync(InitiateCallRequest request, string callerUsername)
    {
        // TODO: If the caller is already in a call/there is a call waiting for him, return an error/handle it nicely.
        // TODO: If A is calling B and B didn't answer yet, C can call B and fuck A's call.
        // TODO: Add a timeout to calls on the server side.

        pendingCalls[request.CalleeUserName] = new ConnectionDetails(request.IPv4Address, request.MinPort,
            request.MaxPort, callerUsername, request.PublicKey);

        while (true)
        {
            await Task.Delay(50);
            if (acceptedCalls.TryRemove(request.CalleeUserName, out var connectionDetails))
                return connectionDetails;
        }
    }

    public IncomingCall? PollForIncomingCall(string userName)
    {
        return pendingCalls.TryGetValue(userName, out var connectionDetails) 
            ? new IncomingCall(connectionDetails.CallerUsername) 
            : null;
    }

    public ConnectionDetails AcceptIncomingCall(AcceptCallRequest request, string userName)
    {
        if (!pendingCalls.TryRemove(userName, out var connectionDetails))
            throw new Exception("No pending call");

        acceptedCalls[userName] = new ConnectionDetails(request.IPv4Address, request.MinPort, request.MaxPort, userName,
            request.PublicKey);

        return connectionDetails;
    }
}