using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Moq;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using uMocks.Builders;
using uMocks.Builders.Abstract;
using uMocks.Exceptions;

namespace uMocks
{
  public class PublishedContentMockSession
  {
    private readonly Mock<IFactory> _factoryMock = new Mock<IFactory>();

    public IPublishedContentMockBuilder PublishedContentBuilder { get; }

    public IGridEditorBuilder GridEditorBuilder { get; }

    private PublishedContentMockSession()
    {
      PublishedContentBuilder = new PublishedContentMockBuilder();
      GridEditorBuilder = new GridEditorBuilder();

      TryInitializeFactory();
    }

    public PublishedContentMockSession WithUmbracoService<TService>(TService serviceInstance)
    {
      _factoryMock.Setup(c => c.GetInstance(typeof(TService))).Returns(serviceInstance);
      _factoryMock.Setup(c => c.TryGetInstance(typeof(TService))).Returns(serviceInstance);

      return this;
    }

    /// <summary>
    /// Configures Umbraco service of requested type as a default Umbraco provided instance
    /// </summary>
    /// <typeparam name="TService">Requested type of service to be configured</typeparam>
    /// <remarks>
    /// Only following types are supported:
    /// - IImageUrlGenerator
    /// </remarks>
    public PublishedContentMockSession WithDefaultUmbracoService<TService>()
      where TService : IImageUrlGenerator
    {
      var serviceInstance = UmbracoDefaultServiceFactory.CreateService<TService>();

      _factoryMock.Setup(c => c.GetInstance(typeof(TService))).Returns(serviceInstance);
      _factoryMock.Setup(c => c.TryGetInstance(typeof(TService))).Returns(serviceInstance);

      return this;
    }

    public static PublishedContentMockSession CreateNew()
    {
      return new PublishedContentMockSession();
    }

    private void TryInitializeFactory()
    {
      try
      {
        _factoryMock.Setup(c => c.GetInstance(It.IsAny<Type>())).Returns<Type>(x =>
        {
          var setupInfo = GetCustomSetupInfo();
          
          throw new MockNotFoundException(x, setupInfo);
        });

        _factoryMock.Setup(c => c.TryGetInstance(It.IsAny<Type>())).Returns<Type>(x =>
        {
            var setupInfo = GetCustomSetupInfo();

            throw new MockNotFoundException(x, setupInfo);
        });

        Current.Factory = _factoryMock.Object;
      }
      catch
      {
        // swallow - 'Current' is a poor singleton
      }
    }

    private IList GetCustomSetupInfo()
    {
      const string fieldName = "setups";

      var setupCollection = typeof(Mock<IFactory>).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_factoryMock);
      var setups = (IList)setupCollection?.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(setupCollection);

      return setups != null && setups.Count > 0 ? new ArrayList(setups).ToArray().Skip(1).ToArray() : new object[0];
    }
  }
}
