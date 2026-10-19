using System.Text.RegularExpressions;

namespace Plainion.DrawVista;

public class InstanceOptions
{
    private InstanceOptions(string name, int? port, string[] hostArguments)
    {
        Name = name;
        Port = port;
        HostArguments = hostArguments;
    }

    public string Name { get; }
    public int? Port { get; }
    public string[] HostArguments { get; }

    public string DataFolder => Path.Combine(
        Environment.GetEnvironmentVariable("ALLUSERSPROFILE"), GlobalConst.AppFolder, Name);

    public string InputFolder => Path.Combine(DataFolder, GlobalConst.InputDirName);

    public static InstanceOptions Parse(string[] args)
    {
        string name = null;
        int? port = null;
        var hostArguments = new List<string>();

        for (var i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "-name":
                    if (name != null)
                    {
                        throw new ArgumentException("Specify -name only once.");
                    }

                    name = ReadValue(args, ref i);
                    if (!IsValidName(name))
                    {
                        throw new ArgumentException("The -name value must be a valid single Windows folder name.");
                    }
                    break;

                case "-p":
                    if (port.HasValue)
                    {
                        throw new ArgumentException("Specify -p only once.");
                    }

                    var portValue = ReadValue(args, ref i);
                    if (!int.TryParse(portValue, out var parsedPort) || parsedPort is < 0 or > 65535)
                    {
                        throw new ArgumentException("The -p value must be an integer between 0 and 65535.");
                    }

                    port = parsedPort;
                    break;

                default:
                    hostArguments.Add(args[i]);
                    break;
            }
        }

        if (name == null)
        {
            throw new ArgumentException("The -name parameter is required.");
        }

        return new InstanceOptions(name, port, hostArguments.ToArray());
    }

    private static string ReadValue(string[] args, ref int index)
    {
        if (index + 1 >= args.Length || args[index + 1].StartsWith('-'))
        {
            throw new ArgumentException($"The {args[index]} parameter requires a value.");
        }

        return args[++index];
    }

    private static bool IsValidName(string name) =>
        !string.IsNullOrWhiteSpace(name)
        && name.Length <= 255
        && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
        && !name.EndsWith('.')
        && !name.EndsWith(' ')
        && !Regex.IsMatch(name,
            @"^(CON|PRN|AUX|NUL|CONIN\$|CONOUT\$|COM[1-9\u00b9\u00b2\u00b3]|LPT[1-9\u00b9\u00b2\u00b3])(\.|$)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
}
