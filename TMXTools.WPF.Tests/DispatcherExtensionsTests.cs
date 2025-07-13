using System.Collections.ObjectModel;
using System.Windows.Data;

using TMXTools.WPF.Extensions;

namespace TMXTools.WPF.Tests;

public class DispatcherExtensionsTests
{
    /// <summary>This test checks that the ListCollectionView gets created correctly and filters accordingly</summary>
    [Test]
    public void TestListCollectionView()
    {
        string itemA = "A";
        string itemB = "B";
        string[] items = [itemA, itemB];
        ObservableCollection<string> collection = DispatcherExtensions.CreateObservableCollection(items);
        ListCollectionView view = collection.CreateListCollectionView((a) => a as string == itemA);
        view.SafeRefresh();
        foreach (var item in view)
        {
            Console.WriteLine(item);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view, Has.Exactly(1).Items);
            Assert.That(view.GetItemAt(0), Is.EqualTo(itemA));
        }
    }

    /// <summary>This test checks that the ListCollectionView can handle a null filter correctly</summary>
    [Test]
    public void TestListCollectionViewNullFilter()
    {
        string itemA = "A";
        string itemB = "B";
        string[] items = [itemA, itemB];
        ObservableCollection<string> collection = DispatcherExtensions.CreateObservableCollection(items);
        ListCollectionView view = collection.CreateListCollectionView(null);
        view.SafeRefresh();
        foreach (var item in view)
        {
            Console.WriteLine(item);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view, Has.Exactly(2).Items);
            Assert.That(view.GetItemAt(0), Is.EqualTo(itemA));
            Assert.That(view.GetItemAt(1), Is.EqualTo(itemB));
        }
    }
}