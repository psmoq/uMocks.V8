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
    }

    public static PublishedContentMockSession CreateNew()
    {
      return new PublishedContentMockSession();
    }
  }
}
