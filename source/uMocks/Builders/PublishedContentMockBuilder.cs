﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using uMocks.Builders.Abstract;
using uMocks.Syntax;

namespace uMocks.Builders
{
  internal class PublishedContentMockBuilder : IPublishedContentMockBuilder
  {
    private readonly ICollection<PublishedContentSyntax> _contentMocks = new List<PublishedContentSyntax>();

    public IPublishedContentSyntax PrepareNew(string documentTypeAlias, string documentName = null, int documentId = 0)
    {
      if (documentId == 0)
        documentId = new Random().Next(8000, 12000);

      var syntax = new PublishedContentSyntax(this, documentTypeAlias, documentName, documentId);

      _contentMocks.Add(syntax);

      return syntax;
    }

    internal Mock<IPublishedContent> GetMockFor(IPublishedContent content)
    {
      var mock = _contentMocks.FirstOrDefault(c => c.PublishedContentMock.Object == content);
      if (mock == null)
        throw new Exception("Given content does not exist in current mock session");

      return mock.PublishedContentMock;
    }

    private class PublishedContentSyntax : IPublishedContentSyntax
    {
      private readonly PublishedContentMockBuilder _builder;

      internal Mock<IPublishedContent> PublishedContentMock { get; }

      private readonly ICollection<IPublishedProperty> _publishedProperties = new List<IPublishedProperty>();

      public PublishedContentSyntax(PublishedContentMockBuilder builder, string documentTypeAlias, string documentName, int documentId)
      {
        _builder = builder;
        PublishedContentMock = new Mock<IPublishedContent>();
        PublishedContentMock.Setup(c => c.Id).Returns(documentId);
        PublishedContentMock.Setup(c => c.Properties).Returns(_publishedProperties);
        PublishedContentMock.Setup(c => c.CreateDate).Returns(DateTime.Now);
        PublishedContentMock.Setup(c => c.UpdateDate).Returns(DateTime.Now);
        PublishedContentMock.Setup(c => c.Path).Returns(() => "-1," + GetDocumentPath(PublishedContentMock.Object));
        PublishedContentMock.Setup(c => c.Level).Returns(() => GetDocumentLevel(PublishedContentMock.Object));
        PublishedContentMock.Setup(c => c.ItemType).Returns(PublishedItemType.Content);
        PublishedContentMock.Setup(c => c.Name).Returns(() => documentName);

        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(c => c.Alias).Returns(() => documentTypeAlias);

        PublishedContentMock.Setup(c => c.ContentType).Returns(() => contentTypeMock.Object);
      }

      public IPublishedContentSyntax WithProperty<T>(string propertyAlias, T value, string culture = null)
      {
        var propertyMock = new Mock<IPublishedProperty>();

        propertyMock.Setup(p => p.Alias).Returns(propertyAlias);
        propertyMock.Setup(p => p.HasValue(culture, It.IsAny<string>())).Returns(() => value != null);
        propertyMock.Setup(p => p.GetValue(culture, It.IsAny<string>())).Returns(value);
        propertyMock.Setup(p => p.GetSourceValue(culture, It.IsAny<string>())).Returns(value);
        

        var propertyTypeMock = new Mock<IPublishedPropertyType>();

        propertyTypeMock.Setup(p => p.Alias).Returns(propertyAlias);

        propertyMock.Setup(p => p.PropertyType).Returns(() => propertyTypeMock.Object);

        _publishedProperties.Add(propertyMock.Object);

        PublishedContentMock.Setup(c => c.GetProperty(It.Is<string>(x => x == propertyAlias)))
          .Returns(() =>
          {
            return _publishedProperties.First(p => p.PropertyType.Alias == propertyAlias);
          });

        PublishedContentMock.Setup(c => c.GetProperty(It.Is<string>(x => x == propertyAlias)))
          .Returns(() =>
          {
            return _publishedProperties.First(p => p.PropertyType.Alias == propertyAlias);
          });

        return this;
      }

      public IPublishedContentSyntax WithChildren(params IPublishedContent[] children)
      {
        foreach (var child in children)
          _builder.GetMockFor(child).Setup(c => c.Parent).Returns(PublishedContentMock.Object);

        PublishedContentMock.Setup(c => c.Children).Returns(() => children);
        PublishedContentMock.Setup(c => c.ChildrenForAllCultures).Returns(() => children);

        return this;
      }

      public IPublishedContentSyntax WithChildren(Func<IPublishedContentMockBuilder, IPublishedContentSyntax[]> childrenSyntaxFunc)
      {
        var children = childrenSyntaxFunc(_builder)
          .Select(child => child.Build())
          .ToArray();

        return WithChildren(children);
      }

      public IPublishedContentSyntax OfParent(IPublishedContent parent)
      {
        var parentMock = _builder.GetMockFor(parent);
        var currentChildren = parentMock.Object.Children.ToList();

        currentChildren.Add(PublishedContentMock.Object);

        parentMock.Setup(c => c.Children).Returns(currentChildren);
        parentMock.Setup(c => c.ChildrenForAllCultures).Returns(currentChildren);
        PublishedContentMock.Setup(c => c.Parent).Returns(parent);

        return this;
      }

      public IPublishedContentSyntax CreatedAt(DateTime createDate)
      {
        PublishedContentMock.Setup(c => c.CreateDate).Returns(createDate);

        return this;
      }

      public IPublishedContentSyntax CreatedBy(string name, int creatorId = 0)
      {
        PublishedContentMock.Setup(c => c.CreatorName).Returns(name);
        PublishedContentMock.Setup(c => c.CreatorId).Returns(creatorId);

        return this;
      }

      public IPublishedContentSyntax UpdatedAt(DateTime updateDate)
      {
        PublishedContentMock.Setup(c => c.UpdateDate).Returns(updateDate);

        return this;
      }

      public IPublishedContentSyntax WrittenBy(string name, int writerId = 0)
      {
        PublishedContentMock.Setup(c => c.WriterName).Returns(name);
        PublishedContentMock.Setup(c => c.WriterId).Returns(writerId);

        return this;
      }

      public IPublishedContentSyntax WithUrl(string url)
      {
        PublishedContentMock.Setup(c => c.Url).Returns(url);

        return this;
      }

      public IPublishedContent Build()
      {
        return PublishedContentMock.Object;
      }

      private string GetDocumentPath(IPublishedContent content)
      {
        var path = string.Empty;
        if (content.Parent != null)
          path += $"{GetDocumentPath(content.Parent)},";

        path += $"{content.Id}";

        return path;
      }

      private int GetDocumentLevel(IPublishedContent content)
      {
        return content.Path.Count(f => f == ',') + 1;
      }
    }
  }
}
