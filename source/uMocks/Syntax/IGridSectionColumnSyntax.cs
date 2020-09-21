using System.Collections.Generic;

namespace uMocks.Syntax
{
  public interface IGridSectionColumnSyntax : IGridSectionSyntax
  {
    IGridSectionColumnSyntax ConfigureColumn(int columnIndex, IDictionary<string, string> configItems);

    IGridSectionColumnSyntax ApplyColumnStyles(int columnIndex, IDictionary<string, string> styleItems);
  }
}
