using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SVA.Infrastructure.Extensions
{
    public static class WPFExtensions
    {
        public static void RefreshDataContext(this FrameworkElement frameworkElement)
        {
            var dc = frameworkElement.DataContext;
            frameworkElement.DataContext = null;
            frameworkElement.DataContext = dc;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            return FindParent<T>(parentObject);
        }

        public static IList<T> FindChildren<T>(this DependencyObject obj) where T : DependencyObject
        {
            var accumulator = new List<T>();
            FindChildren(obj, accumulator);
            return accumulator;
        }

        public static void FindChildren<T>(this DependencyObject obj, IList<T> accumulator) where T : DependencyObject
        {
            obj.FindChildren<T>(accumulator, v => true);
        }

        public static IList<T> FindChildren<T>(this DependencyObject obj, Predicate<T> predicate) where T : DependencyObject
        {
            var accumulator = new List<T>();
            FindChildren(obj, accumulator, predicate);
            return accumulator;
        }

        public static void FindChildren<T>(this DependencyObject obj, IList<T> accumulator, Predicate<T> predicate) where T : DependencyObject
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "DependencyObject reference mustn't be null.");
            }
            if (accumulator == null)
            {
                throw new ArgumentNullException(nameof(accumulator), "List reference mustn't be null.");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate reference mustn't be null.");
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && predicate((T)child))
                {
                    accumulator.Add((T)child);
                }
                FindChildren<T>(child, accumulator, predicate);
            }
            var contentControl = obj as ContentControl;
            if (contentControl?.Content is T && predicate((T) contentControl.Content))
            {
                accumulator.Add((T)contentControl.Content);
            }
            if (contentControl?.Content is DependencyObject)
            {
                FindChildren<T>((DependencyObject)contentControl.Content, accumulator, predicate);
            }
        }
    }
}