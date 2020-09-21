using System.Linq;
using Newtonsoft.Json.Linq;

namespace uMocks.Extensions
{
  public static class GridEditorExtension
  {
    public static JObject[] GetControls(this JObject gridEditor, string componentAlias)
    {
      return gridEditor.SelectTokens($"$..controls[?(@.editor.alias=='{componentAlias}')]").Select(token => (JObject)token).ToArray();
    }

    public static JObject GetControl(this JObject gridEditor, int section, int row, int column, string componentAlias)
    {
      return (JObject)gridEditor.SelectToken($"$.sections[{section}].rows[{row}].areas[{column}].controls[?(@.editor.alias=='{componentAlias}')]");
    }

    public static JObject ExtractValue(this JObject gridEditorComponent)
    {
      return (JObject)gridEditorComponent.SelectToken($"$.value.value");
    }
  }
}
