using uMocks.Syntax;

namespace uMocks.Builders.Abstract
{
  public interface IPublishedContentMockBuilder
  {
    IPublishedContentSyntax PrepareNew(string documentTypeAlias, string documentName = null, int documentId = 0);
  }
}
