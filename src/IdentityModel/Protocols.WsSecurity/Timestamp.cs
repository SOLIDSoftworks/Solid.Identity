using System;

namespace Solid.IdentityModel.Protocols.WsSecurity;

public class Timestamp
{
    public string? Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}