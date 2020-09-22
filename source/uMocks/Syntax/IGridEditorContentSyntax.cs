using System;
using Json.Fluently.Builders.Abstract;
using Newtonsoft.Json.Linq;

namespace uMocks.Syntax
{
  public interface IGridEditorContentSyntax : IGridEditorSyntax
  {
    IGridEditorContentSyntax PutGridComponent(int sectionIndex, int rowIndex, int columnIndex, string alias, JObject value);

    IGridEditorContentSyntax PutGridComponent(int sectionIndex, int rowIndex, int columnIndex,
      string alias, Func<IFluentJsonBuilder, IJsonObjectBuilder> objectSyntaxFunc);
  }
}
