
namespace HeavyKeyProcessing {
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
       public SomeControlViewModel SomeControlViewModel { get; set; } = new SomeControlViewModel();
    }
}