using uMocks.Syntax;

namespace uMocks.Builders.Abstract
{
  public interface IGridEditorBuilder
  {
    IGridEditorLayoutSyntax CreateNew(string layoutName);
  }
}
