namespace ExMDBUtils
{
    using Exiled.API.Interfaces;
    using ExMDBUtils.Settings;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }


        public BaseSettings ScpSelect { get; set; } = new BaseSettings();
        public BaseSettings PlayerAssist { get; set; } = new BaseSettings();
    }
}
