public partial class SystemLocator
{
    private MailManager _mailManager;
    public MailManager MailManager =>
        _mailManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.MailManager] as MailManager;
}
