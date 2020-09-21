using uMocks.Syntax;

namespace uMocks.Builders.Abstract
{
  public interface IPublishedContentMockBuilder
  {
    IPublishedContentSyntax PrepareNew(string documentTypeAlias, int documentId = 0);
  }
}
