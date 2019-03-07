using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WebListener
{
    class ScrollToBottomOnAddBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            INotifyCollectionChanged itemCollection = AssociatedObject.Items;
            itemCollection.CollectionChanged += ItemCollectionOnCollectionChanged;
        }

        private void ItemCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset) return; // Reset приходит при начальной инициализации Rows и обрабатывается в ScrollToPreviousExitPointOrBottomOnLoadBehavior
            if (AssociatedObject.Items.Count == 0) return;

            if (AssociatedObject.SelectedIndex == -1)
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[AssociatedObject.Items.Count - 1]);
            else
            { // если при загрузке мы возвращаемся не к последней записи, то желательно ее поставить в середину таблицы
                int itemNumber = AssociatedObject.SelectedIndex + 1;
                if (itemNumber > AssociatedObject.Items.Count - 1) itemNumber = AssociatedObject.Items.Count - 1;
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[itemNumber]);
            }
        }
    }
}
