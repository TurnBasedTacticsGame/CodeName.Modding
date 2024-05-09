using UnityEngine;

namespace CodeName.Modding.Editor
{
    public interface ISaveableImporter
    {
        public void Save(Object asset, string path);
    }
}
