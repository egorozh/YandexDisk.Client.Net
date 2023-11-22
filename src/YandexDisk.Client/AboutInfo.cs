using System.Linq;
using System.Reflection;

namespace YandexDisk.Client;


/// <summary>
/// Class provided assymbly and file description
/// </summary>
public class AboutInfo
{
    private readonly Assembly _assembly;
    private string _productTitle;
    private static string _version;

    /// <param name="assembly">Assembly for information providing</param>
    public AboutInfo(Assembly assembly)
    {
        _assembly = assembly;
    }

    /// <summary>
    /// Return product title from AssemblyTitleAttribute
    /// </summary>
    public string ProductTitle => _productTitle ?? (_productTitle = GetAttribute<AssemblyTitleAttribute>().Title);

    /// <summary>
    /// Return version of assembly
    /// </summary>
    public string Version => _version ?? (_version = _assembly.GetName().Version.ToString());

    private TAttr GetAttribute<TAttr>()
    {
        return (TAttr)_assembly.GetCustomAttributes(typeof(TAttr), true).First();
    }

    /// <summary>
    /// Default Info for IDiskApi
    /// </summary>
    public static readonly AboutInfo Client = new(typeof(IDiskApi).Assembly);
}