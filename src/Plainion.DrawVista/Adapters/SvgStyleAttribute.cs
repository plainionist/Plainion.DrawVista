namespace Plainion.DrawVista.Adapters;

public class SvgStyleAttribute(string value)
{
    private readonly IDictionary<string, string> myValues =
        value == null ? [] : value.Split(";")
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Split(':')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)))
            .ToDictionary(x => x.First(), x => x.Last());

    public string this[string key]
    {
        get { return myValues[key]; }
        set { myValues[key] = value; }
    }

    public override string ToString() =>
        string.Join(";", myValues.Select(x => x.Key + ": " + x.Value));
}