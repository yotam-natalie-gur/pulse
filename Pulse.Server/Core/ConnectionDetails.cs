﻿namespace Pulse.Server.Core;

public record ConnectionDetails(
    string RemoteIPAddress,
    int MinPort,
    int MaxPort,
    string CallerUsername,
    byte[] PublicKey,
    byte[] IV);