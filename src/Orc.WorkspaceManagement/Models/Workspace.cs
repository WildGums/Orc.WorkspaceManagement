namespace Orc.WorkspaceManagement;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Catel;
using Catel.Configuration;
using Catel.Data;
using Catel.Logging;
using Catel.Runtime.Serialization;

public class Workspace : DynamicConfiguration, IWorkspace, IEqualityComparer<Workspace>
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private static readonly HashSet<string> IgnoredProperties = new(new[]
    {
        nameof(Title),
        nameof(DisplayName),
        nameof(WorkspaceGroup),
        nameof(Persist),
        nameof(CanEdit),
        nameof(CanDelete),
        nameof(IsVisible),
        nameof(Scope),
        nameof(Tag),
        nameof(IsReadOnly),
        nameof(IsDirty),
    });

    private bool _updatingDisplayName;
    private bool _isDirty;

    public Workspace() 
        : this(string.Empty)
    {
            
    }

    public Workspace(string title)
    {
        Title = title;
        Persist = true;
        CanEdit = true;
        CanDelete = true;
        IsVisible = true;
        IsDirty = false;
    }

    public string Title { get; set; }
    public string? DisplayName { get; set; }
    public string? WorkspaceGroup { get; set; }

    public bool Persist { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool IsVisible { get; set; }

    public new bool IsDirty
    {
        get { return _isDirty; }
        private set
        {
            if(Equals(_isDirty, value))
            {
                return;
            }

            _isDirty = value;
            RaisePropertyChanged(nameof(IsDirty));

            UpdateDisplayName();
        }
    }

    [ExcludeFromSerialization]
    public object? Scope { get; set; }

    [ExcludeFromSerialization]
    public object? Tag { get; set; }

    public void ClearWorkspaceValues()
    {
        var workspaceValueNames = GetAllWorkspaceValueNames();

        foreach (var workspaceValueName in workspaceValueNames)
        {
            SetWorkspaceValue(workspaceValueName, null);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_updatingDisplayName)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(e.PropertyName))
        {
            return;
        }

        if (string.Equals(e.PropertyName, nameof(Title)) ||
            string.Equals(e.PropertyName, nameof(IsDirty)))
        {
            UpdateDisplayName();
        }

        if (IgnoredProperties.Contains(e.PropertyName))
        {
            base.OnPropertyChanged(e);
            return;
        }

        IsDirty = true;
    }

    protected override void OnDeserialized()
    {
        base.OnDeserialized();

        UpdateDisplayName();
    }

    private void UpdateDisplayName()
    {
        _updatingDisplayName = true;

        try
        {
            DisplayName = IsDirty
                ? $"{Title}*"
                : Title;
        }
        finally
        {
            _updatingDisplayName = false;
        }
    }

    public List<string> GetAllWorkspaceValueNames()
    {
        var valueNames = new List<string>();

        var propertyData = PropertyDataManager.Default.GetCatelTypeInfo(GetType());

        foreach (var catelProperty in propertyData.GetCatelProperties())
        {
            if (catelProperty.Value.IsModelBaseProperty)
            {
                continue;
            }

            if (IgnoredProperties.Contains(catelProperty.Key))
            {
                continue;
            }

            valueNames.Add(catelProperty.Key);
        }

        return valueNames;
    }

    public void UpdateIsDirtyFlag(bool isDirty)
    {
        IsDirty = isDirty;
    }

    public void SetWorkspaceValue(string name, object? value)
    {
        SetConfigurationValue(name, value);
    }

    public T GetWorkspaceValue<T>(string name, T defaultValue)
    {
        if (!IsConfigurationValueSet(name))
        {
            return defaultValue;
        }

        try
        {
            var value = GetConfigurationValue(name);
            if (value is T)
            {
                return (T)value;
            }

            if (value is null)
            {
                return defaultValue;
            }

            // Cast if necessary
            if (value is string stringValue)
            {
                var convertedType = StringToObjectHelper.ToRightType(typeof(T), stringValue);
                if (convertedType is T correctConvertedType)
                {
                    return correctConvertedType;
                }
            }

            Log.Warning($"Value '{value}' for workspace '{DisplayName}' could not be converted to '{typeof(T).Name}', returning default value");

            return defaultValue;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    private bool Equals(Workspace? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(WorkspaceGroup, other.WorkspaceGroup)
            && string.Equals(Title, other.Title)
            && Equals(Scope, other.Scope);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Workspace) obj);
    }

    public override string ToString()
    {
        return Title;
    }

    public bool Equals(Workspace? x, Workspace? y)
    {
        return x?.Equals(y)??y is null;
    }

    public int GetHashCode(Workspace obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() ^ 17;
    }
}
