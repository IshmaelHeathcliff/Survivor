using System.Collections.Generic;

namespace XYZRPGSystem.Data.SaveLoad
{
    public class DataPersistUtility : IUtility
    {
        readonly HashSet<IDataPersistable> _dataPersisters = new();
        readonly Dictionary<string, PersistableData> _store = new();

        public void RegisterPersister(IDataPersistable persister)
        {
            string dataTag = persister.DataTag;
            if (!string.IsNullOrEmpty(dataTag))
            {
                _dataPersisters.Add(persister);
            }
        }

        public void UnregisterPersisters(IDataPersistable persister)
        {
            _dataPersisters.Remove(persister);
        }


        public void ClearPersisters()
        {
            _dataPersisters.Clear();
        }

        public void Save(object saveObject, string fileName, string folderName)
        {
            SaveLoadManager.Save(saveObject, fileName, folderName);

        }

        public T Load<T>(string fileName, string folderName)
        {
            return SaveLoadManager.Load<T>(fileName, folderName);
        }

        public void SetDirty(IDataPersistable dp)
        {
            Save(dp);
        }

        void SaveAllData()
        {
            foreach (IDataPersistable dp in _dataPersisters)
            {
                Save(dp);
            }
        }


        void Save(IDataPersistable dp)
        {
            if (!string.IsNullOrEmpty(dp.DataTag))
            {
                _store[dp.DataTag] = dp.SaveData();
            }
        }

        void LoadAllData()
        {
            foreach (IDataPersistable dp in _dataPersisters)
            {
                if (!string.IsNullOrEmpty(dp.DataTag))
                {
                    if (_store.TryGetValue(dp.DataTag, out PersistableData data))
                    {
                        dp.LoadData(data);
                    }
                }
            }
        }

        public void SaveAllDataToFile()
        {
            SaveAllData();
            var dataToSave = new Dictionary<string, PersistableData>();
            foreach (IDataPersistable dp in _dataPersisters)
            {
                if (!string.IsNullOrEmpty(dp.DataTag))
                {
                    if (_store.TryGetValue(dp.DataTag, out PersistableData data))
                    {
                        dataToSave[dp.DataTag] = data;
                    }
                }
            }

            SaveLoadManager.Save(dataToSave, "save.json");
        }

        public void LoadAllDataFromFile()
        {
            Dictionary<string, PersistableData> data = SaveLoadManager.Load<Dictionary<string, PersistableData>>("save.json");
            foreach ((string k, PersistableData d) in data)
            {
                _store[k] = d;
            }

            LoadAllData();
        }
    }
}
