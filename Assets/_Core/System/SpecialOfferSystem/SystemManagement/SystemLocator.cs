public partial class SystemLocator
{
    private SpecialOfferManager _specialOfferManager;
    public SpecialOfferManager SpecialOfferManager => _specialOfferManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.SpecialOfferManager] as SpecialOfferManager;
}
