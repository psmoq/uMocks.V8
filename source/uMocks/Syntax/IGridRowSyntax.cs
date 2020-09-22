using System.Collections.Generic;

namespace uMocks.Syntax
{
  public interface IGridRowSyntax
  {
    IGridSectionColumnSyntax WithColumns(int columnCount);

    IGridRowSyntax WithConfig(IDictionary<string, string> configItems);

    IGridRowSyntax WithStyles(IDictionary<string, string> configItems);
  }
}
