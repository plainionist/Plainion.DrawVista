namespace Plainion.DrawVista.Tests;

[TestFixture]
public class InstanceOptionsTests
{
    [Test]
    public void NameIsMandatory()
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse([]));

        Assert.That(exception.Message, Does.Contain("-name"));
    }

    [Test]
    public void PortDoesNotReplaceMandatoryName()
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse(["-p", "5237"]));

        Assert.That(exception.Message, Does.Contain("-name"));
    }

    [TestCase("-name")]
    [TestCase("-p")]
    public void ParameterRequiresAValue(string parameter)
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse([parameter]));

        Assert.That(exception.Message, Does.Contain(parameter));
    }

    [Test]
    public void AnotherOptionCannotBeUsedAsName()
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse(["-name", "-p", "5237"]));

        Assert.That(exception.Message, Does.Contain("-name"));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(".")]
    [TestCase("..")]
    [TestCase(@"..\other")]
    [TestCase(@"team\other")]
    [TestCase("team/other")]
    [TestCase(@"C:\other")]
    [TestCase("team:other")]
    [TestCase("team*")]
    [TestCase("team?")]
    [TestCase("team\"")]
    [TestCase("team.")]
    [TestCase("team ")]
    [TestCase("CON")]
    [TestCase("con.txt")]
    [TestCase("NUL")]
    [TestCase("COM1")]
    [TestCase("lpt9.txt")]
    public void InvalidFolderNameIsRejected(string name)
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse(["-name", name]));

        Assert.That(exception.Message, Does.Contain("-name"));
    }

    [Test]
    public void FolderNameCannotExceedWindowsLimit()
    {
        Assert.Throws<ArgumentException>(() => InstanceOptions.Parse(["-name", new string('x', 256)]));
    }

    [TestCase("team-a")]
    [TestCase("Team with spaces")]
    [TestCase("team;docs")]
    public void ValidNameIsAccepted(string name)
    {
        var options = InstanceOptions.Parse(["-name", name]);

        Assert.That(options.Name, Is.EqualTo(name));
        Assert.That(options.Port, Is.Null);
        Assert.That(options.HostArguments, Is.Empty);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void NameAndPortCanAppearInEitherOrder(bool nameFirst)
    {
        var options = InstanceOptions.Parse(nameFirst
            ? ["-name", "team-a", "-p", "5237"]
            : ["-p", "5237", "-name", "team-a"]);

        Assert.That(options.Name, Is.EqualTo("team-a"));
        Assert.That(options.Port, Is.EqualTo(5237));
        Assert.That(options.HostArguments, Is.Empty);
    }

    [TestCase("0", 0)]
    [TestCase("65535", 65535)]
    public void ValidPortBoundariesAreAccepted(string value, int port)
    {
        var options = InstanceOptions.Parse(["-name", "team-a", "-p", value]);

        Assert.That(options.Port, Is.EqualTo(port));
    }

    [TestCase("-1")]
    [TestCase("65536")]
    [TestCase("not-a-port")]
    public void InvalidPortIsRejected(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => InstanceOptions.Parse(["-name", "team-a", "-p", value]));

        Assert.That(exception.Message, Does.Contain("-p"));
    }

    [Test]
    public void DuplicateNamesAreRejected()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            InstanceOptions.Parse(["-name", "team-a", "-name", "team-b"]));

        Assert.That(exception.Message, Does.Contain("-name"));
    }

    [Test]
    public void DuplicatePortsAreRejected()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            InstanceOptions.Parse(["-name", "team-a", "-p", "5237", "-p", "5238"]));

        Assert.That(exception.Message, Does.Contain("-p"));
    }

    [Test]
    public void HostArgumentsArePreserved()
    {
        var options = InstanceOptions.Parse([
            "--HostUri", "http://localhost:5237", "-name", "team-a",
            "--environment", "Development", "-p", "5238"]);

        Assert.That(options.HostArguments,
            Is.EqualTo(new[] { "--HostUri", "http://localhost:5237", "--environment", "Development" }));
    }

    [TestCase("team-a")]
    [TestCase("Team with spaces")]
    public void StorageFoldersAreScopedByName(string name)
    {
        var options = InstanceOptions.Parse(["-name", name]);
        var expectedFolder = Path.Combine(
            Environment.GetEnvironmentVariable("ALLUSERSPROFILE"), "Plainion.DrawVista", name);

        Assert.That(options.DataFolder, Is.EqualTo(expectedFolder));
        Assert.That(options.InputFolder, Is.EqualTo(Path.Combine(expectedFolder, "input")));
    }

    [Test]
    public void DifferentNamesHaveSeparateStorageFolders()
    {
        var first = InstanceOptions.Parse(["-name", "team-a"]);
        var second = InstanceOptions.Parse(["-name", "team-b"]);

        Assert.That(first.DataFolder, Is.Not.EqualTo(second.DataFolder));
        Assert.That(first.InputFolder, Is.Not.EqualTo(second.InputFolder));
    }
}
