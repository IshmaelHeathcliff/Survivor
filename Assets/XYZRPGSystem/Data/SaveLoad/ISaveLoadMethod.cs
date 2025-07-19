using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XYZRPGSystem.Data.SaveLoad
{
    public interface ISaveLoadMethod
    {
        void Save(object saveObject, FileStream saveFile);

        T Load<T>(FileStream saveFile);
    }

    public enum SaveLoadMethods { Json };
}
