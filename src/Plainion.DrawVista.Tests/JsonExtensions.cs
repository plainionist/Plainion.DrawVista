using Newtonsoft.Json;

namespace Plainion.DrawVista.Tests;

public static class JsonExtensions
{
    public static string ToJson(this object self) =>
        JsonConvert.SerializeObject(self);
}
