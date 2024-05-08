namespace CodeName.Modding.Mods
{
    public class LoadedModInfo
    {
        public LoadedModInfo(ModInfo originalInfo)
        {
            OriginalInfo = originalInfo;
        }

        public ModInfo OriginalInfo { get; }
    }
}
