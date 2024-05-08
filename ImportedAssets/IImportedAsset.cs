namespace CodeName.Modding.ImportedAssets
{
    public interface IImportedAsset
    {
#if UNITY_EDITOR
        public void Save(string path);
#endif
    }
}
