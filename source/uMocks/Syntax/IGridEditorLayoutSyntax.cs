namespace uMocks.Syntax
{
  public interface IGridEditorLayoutSyntax : IGridEditorSyntax
  {
    IGridSectionSyntax AddSection(int layoutColumnCount);

    IGridEditorContentSyntax SubmitLayout();
  }
}
