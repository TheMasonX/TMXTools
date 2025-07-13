using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;


namespace TMXTools.WPF.Extensions;

public static class DispatcherExtensions
{

    /// <summary>
    /// Creates an <see cref="ObservableCollection{T}"/> on the UI thread.
    /// </summary>
    /// <typeparam name="T">The type of elements within the collection</typeparam>
    /// <param name="collection"><see cref="IEnumerable{T}"/> default elements to be added</param>
    /// <param name="invokeIfNoDispatcher">Should the collection be created on the calling thread if no application dispatcher is available?</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> on the UI thread</returns>
    public static ObservableCollection<T> CreateObservableCollection<T>(IEnumerable<T>? collection = null, bool invokeIfNoDispatcher = true)
    {
        ObservableCollection<T> result = null!;
        SafeInvoke(() => result = new ObservableCollection<T>(collection ?? []), invokeIfNoDispatcher);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="ListCollectionView"/> wrapping an <see cref="ObservableCollection{T}"/> on the UI thread.
    /// <br/>
    /// Useful for filtering and sorting collections in WPF.
    /// </summary>
    /// <param name="collection"><see cref="ObservableCollection{T}"/> that this will wrap as a view</param>
    /// <param name="filter">Predicate to filter the items in the collection. Can be <see langword="null"/> if no filtering is needed.</param>
    /// <param name="invokeIfNoDispatcher">Should the collection be created on the calling thread if no application dispatcher is available?</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> on the UI thread</returns>
    public static ListCollectionView CreateListCollectionView<T>(this ObservableCollection<T> collection, Predicate<object>? filter = null, bool invokeIfNoDispatcher = true)
    {
        ListCollectionView result = null!;
        SafeInvoke(() => result = new ListCollectionView(collection) { Filter = filter }, invokeIfNoDispatcher);
        return result;
    }

    /// <summary>
    /// Invokes an <see cref="Action"/> on the UI thread if the application has a dispatcher.
    /// </summary>
    /// <param name="action">Action to be invoked</param>
    /// <param name="invokeIfNoDispatcher">Should the action still be invoked on the calling thread if no application dispatcher is available?</param>
    /// <param name="actionName">
    /// Used by the compiler to insert the name from the calling site.
    /// Don't use if not familiar with <see cref="CallerArgumentExpressionAttribute"/>.
    /// </param>
    /// <returns><see langword="true"/> if the invoke was successful; Otherwise, <see langword="false"/>.</returns>
    public static bool SafeInvoke(Action action, bool invokeIfNoDispatcher = true, [CallerArgumentExpression(nameof(action))] string actionName = "")
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            if (!invokeIfNoDispatcher)
            {
                Trace.TraceWarning($"Application.Current.Dispatcher is null, can't invoke {actionName}");
                //Log.Logger.Warning("Application.Current.Dispatcher is null, can't invoke {Action}", actionName);
                return false;
            }
            Trace.TraceWarning($"Application.Current.Dispatcher is null, but we'll try invoking anyways {actionName}");
            //Log.Logger.Warning("Application.Current.Dispatcher is null, but we'll try invoking anyways {Action}", actionName);
            try
            {
                action.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to invoke action {actionName} on the calling thread: {ex}");
                //Log.Logger.Error(ex, "Failed to invoke action {Action} on the calling thread", actionName);
                return false;
            }
        }

        dispatcher.Invoke(action);
        return true;
    }

    /// <summary>
    /// Invokes a <c>Func&lt;<see langword="bool"/>&gt;</c> on the UI thread if the application has a dispatcher.
    /// Returns the result of the function if the invoke was successful, otherwise <paramref name="defaultResult"/>.
    /// </summary>
    /// <param name="func">Function to be invoked</param>
    /// <param name="defaultResult">Default value to be used in case the invoke fails</param>
    /// <param name="invokeIfNoDispatcher">Should the Func still be invoked on the calling thread if no application dispatcher is available?</param>
    /// <param name="funcName">
    /// Used by the compiler to insert the name from the calling site.
    /// Don't use if not familiar with <see cref="CallerArgumentExpressionAttribute"/>.
    /// </param>
    /// <returns>The result of the <c>Func&lt;<see langword="bool"/>&gt;</c> invoke; Otherwise, <paramref name="defaultResult"/> if the invoke failed.</returns>
    public static bool SafeInvoke(Func<bool> func, bool defaultResult = false, bool invokeIfNoDispatcher = true, [CallerArgumentExpression(nameof(func))] string funcName = "")
    {
        bool res = false;
        void action() => res = func.Invoke(); //Convert the Func into an Action
        if (!SafeInvoke(action, invokeIfNoDispatcher, funcName)) //Forward the funcName to preserve the context information
            return defaultResult;

        return res;
    }

    /// <summary>
    /// Safely adds an item to the specified <see cref="ObservableCollection{T}"/> instance by invoking on the UI thread.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The <see cref="ObservableCollection{T}"/> to which the item will be added. Cannot be <see langword="null"/>.</param>
    /// <param name="model">The item to add to the collection. Can be <see langword="null"/> if the collection allows null values.</param>
    /// <returns><see langword="true"/> if the item was successfully added to the collection; Otherwise, <see langword="false"/>
    /// if the operation failed.</returns>
    public static bool SafeAdd<T>(this ObservableCollection<T> collection, T model) => SafeInvoke(() => collection.Add(model));

    /// <summary>
    /// Safely removes the specified item from the <see cref="ObservableCollection{T}"/> by invoking on the UI thread.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection from which the item will be removed. Cannot be <see langword="null"/>.</param>
    /// <param name="model">The item to remove from the collection.</param>
    /// <returns><see langword="true"/> if the item was successfully removed; Otherwise, <see langword="false"/>.</returns>
    public static bool SafeRemove<T>(this ObservableCollection<T> collection, T model) => SafeInvoke(() => collection.Remove(model));

    /// <summary>
    /// Safely clears the <see cref="ObservableCollection{T}"/> by invoking on the UI thread.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection which will be cleared. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the collection was successfully cleared; Otherwise, <see langword="false"/>.</returns>
    public static bool SafeClear<T>(this ObservableCollection<T> collection) => SafeInvoke(collection.Clear);


    /// <summary>
    /// Safely clears the <see cref="ObservableCollection{T}"/> by invoking on the UI thread.
    /// </summary>
    /// <param name="view">The <see cref="ListCollectionView"/> which will be cleared. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the collection was successfully cleared; Otherwise, <see langword="false"/>.</returns>
    public static void SafeRefresh(this ListCollectionView view) => view.Dispatcher?.Invoke(view.Refresh);
}
