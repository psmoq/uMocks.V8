using System;
using System.Collections.Generic;
using System.Linq;
using Json.Fluently.Builders;
using Json.Fluently.Builders.Abstract;
using Json.Fluently.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using uMocks.Builders.Abstract;
using uMocks.Syntax;

namespace uMocks.Builders
{
  internal class GridEditorBuilder : IGridEditorBuilder
  {
    public IGridEditorLayoutSyntax CreateNew(string layoutName)
    {
      return new GridEditorSyntax(layoutName);
    }

    private class GridEditorSyntax : IGridEditorLayoutSyntax, IGridEditorContentSyntax
    {
      private readonly ICollection<GridSection> _gridSections = new List<GridSection>();

      private readonly string _layoutName;

      public GridEditorSyntax(string layoutName)
      {
        _layoutName = layoutName;
      }

      public IGridEditorContentSyntax SubmitLayout()
      {
        return this;
      }

      public IGridSectionSyntax AddSection(int layoutColumnCount)
      {
        var gridSection = new GridSection(layoutColumnCount);

        _gridSections.Add(gridSection);

        return new GridSectionSyntax(this, gridSection);
      }

      public IGridEditorContentSyntax PutGridComponent(int sectionIndex, int rowIndex, int columnIndex, string alias, JObject value)
      {
        var section = _gridSections.ElementAtOrDefault(sectionIndex);
        if (section == null)
          throw new IndexOutOfRangeException("Given section index is out of range.");

        var row = section.Rows.ElementAtOrDefault(rowIndex);
        if (row == null)
          throw new IndexOutOfRangeException("Given row index is out of range.");

        var column = row.Columns.ElementAtOrDefault(columnIndex);
        if (column == null)
          throw new IndexOutOfRangeException("Given column index is out of range.");

        column.Controls.Add(new GridControl(alias, value));

        return this;
      }

      public IGridEditorContentSyntax PutGridComponent(int sectionIndex, int rowIndex, int columnIndex,
        string alias, Func<IFluentJsonBuilder, IJsonObjectBuilder> objectSyntaxFunc)
      {
        var builder = new FluentJsonBuilder();

        var obj = objectSyntaxFunc(builder).Build();

        return PutGridComponent(sectionIndex, rowIndex, columnIndex, alias, obj);
      }

      public JObject Build()
      {
        var builder = new FluentJsonBuilder();

        var gridObject = builder.CreateNew()
          .WithProperty("name", _layoutName)
          .WithArray("sections", GetGridSections)
          .Build();

        return gridObject;
      }

      private IJsonArrayBuilder GetGridSections(IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(_gridSections.Select(gridSection => builder.CreateNew()
          .WithProperty("grid", gridSection.LayoutColumnCount)
          .WithArray("rows", stx => GetGridSectionRows(gridSection, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridSectionRows(GridSection section, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(section.Rows.Select(gridRow => builder.CreateNew()
          .WithProperty("id", GetComponentId())
          .WithProperty("name", gridRow.LayoutName)
          .WithProperty("hasConfig", gridRow.HasConfig)
          .WithObject("config", GetDictionaryObject(gridRow.Config))
          .WithObject("styles", GetDictionaryObject(gridRow.Styles))
          .WithArray("areas", stx => GetGridRowColumns(gridRow, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridRowColumns(GridRow row, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(row.Columns.Select(gridRowColumn => builder.CreateNew()
          .WithProperty("grid", 12 / row.Columns.Count)
          .WithProperty("allowAll", true) // TODO: parametrize this value later
        //.WithArray("allowed", new JArray()) // TODO: parametrize this value later
          .WithProperty("hasConfig", gridRowColumn.HasConfig)
          .WithObject("config", GetDictionaryObject(gridRowColumn.Config))
          .WithObject("styles", GetDictionaryObject(gridRowColumn.Styles))
          .WithArray("controls", stx => GetGridColumnControls(gridRowColumn, stx))
          .Build()).ToArray());
      }

      private IJsonArrayBuilder GetGridColumnControls(GridColumn column, IJsonArraySyntax arraySyntax)
      {
        var builder = new FluentJsonBuilder();

        return arraySyntax.WithItems(column.Controls.Select(control => builder.CreateNew()
          .WithObject("value", stx => stx.CreateNew()
            .WithProperty("id", GetComponentId())
            .WithProperty("dtgeContentTypeAlias", control.Alias)
            .WithObject("value", control.Value))
          .WithObject("editor", stx => stx.CreateNew()
            .WithProperty("alias", control.Alias))
          .WithProperty("active", true)
          .Build()).ToArray());
      }

      private JObject GetDictionaryObject(IDictionary<string, string> dictionary)
      {
        return  JObject.FromObject(dictionary, new JsonSerializer
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
      }

      private string GetComponentId()
      {
        return Guid.NewGuid().ToString();
      }

      private class GridSectionSyntax : IGridSectionSyntax, IGridSectionColumnSyntax
      {
        private readonly IGridEditorLayoutSyntax _gridEditorLayoutSyntax;

        private readonly GridSection _gridSection;

        public GridSectionSyntax(IGridEditorLayoutSyntax gridEditorLayoutSyntax, GridSection section)
        {
          _gridEditorLayoutSyntax = gridEditorLayoutSyntax;
          _gridSection = section;
        }

        public IGridRowSyntax AddRow(string rowLayoutName)
        {
          var row = new GridRow(rowLayoutName);

          _gridSection.Rows.Add(row);

          return new GridRowSyntax(this, row);
        }

        public IGridSectionColumnSyntax ConfigureColumn(int columnIndex, IDictionary<string, string> configItems)
        {
          var lastAddedRow = _gridSection.Rows.LastOrDefault();
          if (lastAddedRow == null)
            throw new Exception("There is no row defined in section layout. Please add row first.");

          var column = lastAddedRow.Columns.ElementAtOrDefault(columnIndex);
          if (column == null)
            throw new Exception("There is no row column at given index.");

          column.Config.Clear();

          foreach (var configItem in configItems)
            column.Config.Add(configItem);

          return this;
        }

        public IGridSectionColumnSyntax ApplyColumnStyles(int columnIndex, IDictionary<string, string> styleItems)
        {
          var lastAddedRow = _gridSection.Rows.LastOrDefault();
          if (lastAddedRow == null)
            throw new Exception("There is no row defined in section layout. Please add row first.");

          var column = lastAddedRow.Columns.ElementAtOrDefault(columnIndex);
          if (column == null)
            throw new Exception("There is no row column at given index.");

          column.Styles.Clear();

          foreach (var styleItem in styleItems)
            column.Styles.Add(styleItem);

          return this;
        }

        public IGridEditorLayoutSyntax SubmitSection()
        {
          return _gridEditorLayoutSyntax;
        }
      }

      private class GridRowSyntax : IGridRowSyntax
      {
        private readonly IGridSectionColumnSyntax _gridSectionSyntax;

        private readonly GridRow _gridRow;

        public GridRowSyntax(IGridSectionColumnSyntax gridSectionSyntax, GridRow row)
        {
          _gridSectionSyntax = gridSectionSyntax;
          _gridRow = row;
        }

        public IGridRowSyntax WithConfig(IDictionary<string, string> configItems)
        {
          _gridRow.Config.Clear();

          foreach (var configItem in configItems)
            _gridRow.Config.Add(configItem);

          return this;
        }

        public IGridRowSyntax WithStyles(IDictionary<string, string> styleItems)
        {
          _gridRow.Styles.Clear();

          foreach (var styleItem in styleItems)
            _gridRow.Styles.Add(styleItem);

          return this;
        }

        public IGridSectionColumnSyntax WithColumns(int columnCount)
        {
          for (int i = 0; i < columnCount; i++)
            _gridRow.Columns.Add(new GridColumn());

          return _gridSectionSyntax;
        }
      }

      private class GridSection
      {
        public int LayoutColumnCount { get; }

        public ICollection<GridRow> Rows { get; }

        public GridSection(int layoutColumnCount)
        {
          LayoutColumnCount = layoutColumnCount;
          Rows = new List<GridRow>();
        }
      }

      private class GridRow
      {
        public string LayoutName { get;  }

        public bool HasConfig => Config.Any();

        public IDictionary<string, string> Config { get; }

        public IDictionary<string, string> Styles { get; }

        public ICollection<GridColumn> Columns { get; }

        public GridRow(string layoutName)
        {
          LayoutName = layoutName;
          Columns = new List<GridColumn>();
          Config = new Dictionary<string, string>();
          Styles = new Dictionary<string, string>();
        }
      }

      private class GridColumn
      {
        public ICollection<GridControl> Controls { get; }

        public bool HasConfig => Config.Any();

        public IDictionary<string, string> Config { get; }

        public IDictionary<string, string> Styles { get; }

        public GridColumn()
        {
          Controls = new List<GridControl>();
          Config = new Dictionary<string, string>();
          Styles = new Dictionary<string, string>();
        }
      }

      private class GridControl
      {
        public string Alias { get; }

        public JObject Value { get; }

        public GridControl(string alias, JObject value)
        {
          Alias = alias;
          Value = value;
        }
      }
    }
  }
}
