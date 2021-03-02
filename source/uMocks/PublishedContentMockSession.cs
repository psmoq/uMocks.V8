using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Moq;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using uMocks.Builders;
using uMocks.Builders.Abstract;
using uMocks.Exceptions;

namespace uMocks
{
  public class PublishedContentMockSession
  {
    private readonly Mock<IFactory> _factoryMock = new Mock<IFactory>();

    private static PublishedContentMockSession _sessionInstance;

    public IPublishedContentMockBuilder PublishedContentBuilder { get; }

    public IGridEditorBuilder GridEditorBuilder { get; }

    private PublishedContentMockSession()
    {
      PublishedContentBuilder = new PublishedContentMockBuilder();
      GridEditorBuilder = new GridEditorBuilder();

      TryInitializeFactory();
    }

    public static PublishedContentMockSession CreateOrGet()
    {
        if (_sessionInstance == null)
            _sessionInstance = new PublishedContentMockSession();

        return _sessionInstance;
    }

    public PublishedContentMockSession WithUmbracoService<TService>(TService serviceInstance)
    {
      _factoryMock.Setup(c => c.GetInstance(typeof(TService))).Returns(serviceInstance);

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

      return this;
    }

    public void Reset()
    {
        _factoryMock.Invocations.Clear();
        _factoryMock.Reset();

        TryInitializeFactory();
    }

    private void TryInitializeFactory()
    {
      try
      {
        _factoryMock.Setup(c => c.GetInstance(It.IsAny<Type>())).Returns<Type>(x =>
        {
            if (x == typeof(TypeLoader))
                return null; // override type loader to prevent Umbraco from loading types

            if (x == typeof(IVariationContextAccessor))
                return null; // override variation context accessor to prevent Umbraco from searching for cultures

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
