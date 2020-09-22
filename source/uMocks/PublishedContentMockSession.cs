﻿using System;
using Moq;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using uMocks.Builders;
using uMocks.Builders.Abstract;

namespace uMocks
{
  public class PublishedContentMockSession
  {
    public IPublishedContentMockBuilder PublishedContentBuilder { get; }

    public IGridEditorBuilder GridEditorBuilder { get; }

    private PublishedContentMockSession()
    {
      PublishedContentBuilder = new PublishedContentMockBuilder();
      GridEditorBuilder = new GridEditorBuilder();

      var factoryMock = new Mock<IFactory>();
      factoryMock.Setup(c => c.GetInstance(It.IsAny<Type>())).Returns(null);

      Current.Factory = factoryMock.Object;
    }

    public static PublishedContentMockSession CreateNew()
    {
      return new PublishedContentMockSession();
    }
  }
}