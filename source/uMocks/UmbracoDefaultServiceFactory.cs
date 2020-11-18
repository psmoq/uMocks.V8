using System.Reflection;
using Umbraco.Core.Models;

namespace uMocks
{
  public class UmbracoDefaultServiceFactory
  {
    /// <summary>
    /// Creates instance of default Umbraco service type
    /// </summary>
    /// <typeparam name="TService">Requested type of service to be instantiated</typeparam>
    /// <returns>Instance of default Umbraco service type</returns>
    /// <remarks>
    /// Only following types are supported:
    /// - IImageUrlGenerator
    /// </remarks>
    public static TService CreateService<TService>()
      where TService : IImageUrlGenerator
    {
      if (typeof(TService) == typeof(IImageUrlGenerator))
        return (TService) CreateImageUrlGenerator();

      return default;
    }

    private static IImageUrlGenerator CreateImageUrlGenerator()
    {
      var assembly = Assembly.LoadFrom("Umbraco.Web.dll");
      var instance = assembly.CreateInstance("Umbraco.Web.Models.ImageProcessorImageUrlGenerator");

      return (IImageUrlGenerator) instance;
    }
  }
}
