namespace ExMDBUtils
{
    using Exiled.API.Interfaces;
    using ExMDBUtils.Settings;
    using System.ComponentModel;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }

        [Description("Switch the spawned scp\'s for one of a chosen set of roles")]
        public BaseSettings ScpSelect { get; set; } = new BaseSettings();

        [Description("Switch a player to a role they aren\'t, and their switchee to their role.")]
        public BaseSettings PlayerAssist { get; set; } = new BaseSettings();

        [Description("Returns to the original behaviour that makes detained class conversions count towards win conditions")]
        public BaseSettings DetainUse { get; set; } = new BaseSettings();

        [Description("Alerts admins of pesky mod abuse")]
        public BaseSettings CommandWatcher { get; set; } = new BaseSettings();
    }
}
