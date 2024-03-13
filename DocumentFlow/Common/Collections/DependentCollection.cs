//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DocumentFlow.Common.Collections;

public sealed class DependentCollection<T> : ObservableCollection<T>, IDependentCollection
    where T : class, IDependentEntity
{
    private readonly DocumentInfo owner;

    public DependentCollection(DocumentInfo owner, IEnumerable<T> collection) : base(collection)
    {
        this.owner = owner;

        owner.PropertyChanged += Owner_PropertyChanged;
        foreach (var item in collection.OfType<INotifyPropertyChanged>())
        {
            item.PropertyChanged += Changed_PropertyChanged;
        }
    }

    public List<IDependentEntity> NewItems { get; } = new();
    public List<IDependentEntity> UpdateItems { get; } = new();
    public List<IDependentEntity> RemoveItems { get; } = new();

    public void CompleteChanged()
    {
        NewItems.Clear();
        UpdateItems.Clear();
        RemoveItems.Clear();
    }

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);

        if (owner.Id != default)
        {
            item.SetOwner(owner);
        }

        if (item.Id == 0)
        {
            NewItems.Add(item);
        }
        else
        {
            if (item is INotifyPropertyChanged changed)
            {
                changed.PropertyChanged += Changed_PropertyChanged;
            }
            else
            {
                UpdateItems.Add(item);
            }
        }
    }

    protected override void RemoveItem(int index)
    {
        T item = base[index];
        if (item.Id == default)
        {
            if (NewItems.Contains(item))
            {
                NewItems.Remove(item);
            }
        }
        else
        {
            if (UpdateItems.Contains(item))
            { 
                UpdateItems.Remove(item);
            }

            RemoveItems.Add(item);
        }

        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        foreach (var item in Items.Where(x => x.Id != 0))
        {
            RemoveItems.Add(item);
        }

        UpdateItems.Clear();
        NewItems.Clear();
            
        base.ClearItems();
    }

    private void Owner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentInfo.Id))
        {
            foreach (var part in Items)
            {
                part.SetOwner(owner);
            }
        }
    }

    private void Changed_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is IDependentEntity entity)
        {
            if (!UpdateItems.Contains(entity))
            {
                UpdateItems.Add(entity);
            }
        }
    }
}
