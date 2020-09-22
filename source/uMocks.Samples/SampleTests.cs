using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using uMocks.Extensions;

namespace uMocks.Samples
{
  [TestClass]
  public class SampleTests
  {
    [TestMethod]
    public void PublishedContent_ShouldBeMockedProperly()
    {
      // Arrange

      var mockSession = PublishedContentMockSession.CreateNew();
      var doc1 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1001)
        .WithProperty("propAlias", "propValue")
        .Build();

      var doc2 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1002)
        .OfParent(doc1)
        .Build();

      var doc3 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1003)
        .OfParent(doc2)
        .Build();

      var doc4 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1004)
        .OfParent(doc2)
        .Build();

      // Assert

      // getting basic content data are supported in all possible ways
      Assert.AreEqual("documentTypeAlias", doc1.ContentType.Alias);
      Assert.AreEqual("documentTypeAlias", doc1.ContentType.Alias);
      Assert.AreEqual("propValue",doc1.Value<string>("propAlias"));
      Assert.AreEqual("propValue", doc1.Value("propAlias"));
      Assert.AreEqual("propValue", doc1.GetProperty("propAlias").Value());
      Assert.AreEqual("propValue", doc1.Properties.First(c => c.Alias == "propAlias").Value<string>());

      // parent-children relations behave as expected in scope of single mock session - relations are dynamically updated
      Assert.AreEqual("-1,1001,1002,1003", doc3.Path);
      Assert.AreEqual(2, doc2.Children.Count());
      Assert.AreEqual(1002, doc4.Parent.Id);

      // most extension methods evaluates properly (methods independent on HttpContext and ApplicationContext)
      Assert.AreEqual(1, doc3.Siblings().Count());
    }

    [TestMethod]
    public void PublishedContent_ShouldUpdateFamilyTreeDynamically()
    {
      // Arrange

      var mockSession = PublishedContentMockSession.CreateNew();
      var doc1 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1001)
        .WithProperty("propAlias", "propValue")
        .WithChildren(stx => new[] {stx.PrepareNew("anotherDocTypeAlias", documentId: 1002)})
        .Build();

      // Assert

      Assert.AreEqual(1, doc1.Children.Count());
      Assert.AreEqual(1002, doc1.Children.First().Id);

      // Arrange

      var doc2 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1003)
        .OfParent(doc1)
        .Build();

      // Assert

      Assert.AreEqual(2, doc1.Children.Count());
      Assert.AreEqual(2, doc1.Children.OfTypes("documentTypeAlias", "anotherDocTypeAlias").Count());
    }

    [TestMethod]
    public void PublishedContent_ShouldHaveProperDatesSet()
    {
      // Arrange

      var createDate = DateTime.Today.AddDays(-10).AddYears(-1);
      var updateDate = DateTime.Today.AddDays(-10);

      var mockSession = PublishedContentMockSession.CreateNew();
      var doc1 = mockSession.PublishedContentBuilder
        .PrepareNew("documentTypeAlias", documentId: 1001)
        .CreatedAt(createDate)
        .UpdatedAt(updateDate)
        .Build();

      // Assert

      Assert.AreEqual(createDate, doc1.CreateDate);
      Assert.AreEqual(updateDate, doc1.UpdateDate);
    }

    [TestMethod]
    public void GridComponent_ShouldHaveGivenValue()
    {
      // Arrange

      var mockSession = PublishedContentMockSession.CreateNew();
      var gridEditor = mockSession.GridEditorBuilder
        .CreateNew("1 column layout")
        .AddSection(12)
        .AddFullWidthRow(config: new Dictionary<string, string>
        {
          { "rowKey",  "value" }
        }, styles: new Dictionary<string, string>
        {
          { "rowStyle",  "value" }
        })
        .ConfigureColumn(0, new Dictionary<string, string>
        {
          { "columnKey",  "value" }
        })
        .SubmitSection()
        .SubmitLayout()
        .PutGridComponent(sectionIndex: 0, rowIndex: 0, columnIndex: 0, alias: "componentAlias", b => b.CreateNew()
          .WithProperty("propertyName1", "propertyValue1")
          .WithProperty("propertyName2", "propertyValue2"))
        .Build();

      // get controls manually
      var controlTokens = gridEditor.SelectTokens("$..controls[*]").ToArray();

      // or use extension methods
      var gridComponent = gridEditor.GetControl(0, 0, 0, "componentAlias");
      var gridComponentValue = gridComponent.ExtractValue();

      // Assert

      // grid editor should have one component defined
      Assert.AreEqual(1, controlTokens.Length); 

      var control = controlTokens.First();

      // grid editor should have given alias
      Assert.AreEqual("componentAlias", control.SelectToken("$.editor").Value<string>("alias"));

      // grid editor should have given property value
      Assert.AreEqual("propertyValue1", control.SelectToken("$.value.value").Value<string>("propertyName1"));
    }
  }
}
