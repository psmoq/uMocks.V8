using System.Collections.Generic;
using uMocks.Syntax;

namespace uMocks.Extensions
{
  public static class GridBuilderExtension
  {
    public static IGridSectionColumnSyntax AddFullWidthRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("full width");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(1);
    }

    public static IGridSectionColumnSyntax AddHalvedRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("6-6");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(2);
    }

    public static IGridSectionColumnSyntax AddOneThirdRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("4-4-4");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(3);
    }

    public static IGridSectionColumnSyntax AddQuarteredRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("3-3-3-3");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(4);
    }

    public static IGridSectionColumnSyntax AddFourToEightRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("4-8");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(2);
    }

    public static IGridSectionColumnSyntax AddEightToFourRow(this IGridSectionSyntax stx,
      IDictionary<string, string> config = null, IDictionary<string, string> styles = null)
    {
      var row = stx.AddRow("8-4");

      if (config != null)
        row = row.WithConfig(config);

      if (styles != null)
        row = row.WithStyles(styles);

      return row.WithColumns(2);
    }
  }
}
