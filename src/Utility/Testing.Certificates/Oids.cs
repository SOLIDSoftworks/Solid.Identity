using System.Security.Cryptography;

namespace Solid.Testing.Certificates;

public static class Oids
{
    public static Oid ServerAuthentication = new Oid("1.3.6.1.5.5.7.3.1");
    public static Oid ClientAuthentication = new Oid("1.3.6.1.5.5.7.3.2");
}