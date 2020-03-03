using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WebListener
{
    class ScrollToBottomDatagridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += ScrollToBottom;
            AssociatedObject.SizeChanged += ScrollToBottom;
        }

        private void ScrollToBottom(object sender, RoutedEventArgs routedEventArgs)
        {
            if (AssociatedObject.Items.Count > 0)
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[AssociatedObject.Items.Count - 1]);
        }
    }
}
