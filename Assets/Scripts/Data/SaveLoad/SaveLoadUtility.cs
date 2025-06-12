using System.Collections.Generic;

namespace Data.SaveLoad
{
    public class SaveLoadUtility : IUtility
    {
        readonly HashSet<ISaveData> _dataPersisters = new HashSet<ISaveData>();
        readonly Dictionary<string, global::Data.SaveLoad.Data> _store = new Dictionary<string, global::Data.SaveLoad.Data>();

        public void RegisterPersister(ISaveData persister)
        {
            string dataTag = persister.DataTag;
            if (!string.IsNullOrEmpty(dataTag))
            {
                _dataPersisters.Add(persister);
            }
        }

        public void UnregisterPersisters(ISaveData persister)
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

        public void SetDirty(ISaveData dp)
        {
            Save(dp);
        }

        void SaveAllData()
        {
            foreach (ISaveData dp in _dataPersisters)
            {
                Save(dp);
            }
        }


        void Save(ISaveData dp)
        {
            if (!string.IsNullOrEmpty(dp.DataTag))
            {
                _store[dp.DataTag] = dp.SaveData();
            }
        }

        void LoadAllData()
        {
            foreach (ISaveData dp in _dataPersisters)
            {
                if (!string.IsNullOrEmpty(dp.DataTag))
                {
                    if (_store.TryGetValue(dp.DataTag, out global::Data.SaveLoad.Data data))
                    {
                        dp.LoadData(data);
                    }
                }
            }
        }

        public void SaveAllDataToFile()
        {
            SaveAllData();
            var dataToSave = new Dictionary<string, global::Data.SaveLoad.Data>();
            foreach (ISaveData dp in _dataPersisters)
            {
                if (!string.IsNullOrEmpty(dp.DataTag))
                {
                    if (_store.TryGetValue(dp.DataTag, out global::Data.SaveLoad.Data data))
                    {
                        dataToSave[dp.DataTag] = data;
                    }
                }
            }

            SaveLoadManager.Save(dataToSave, "save.json");
        }

        public void LoadAllDataFromFile()
        {
            Dictionary<string, global::Data.SaveLoad.Data> data = SaveLoadManager.Load<Dictionary<string, global::Data.SaveLoad.Data>>("save.json");
            foreach ((string k, global::Data.SaveLoad.Data d) in data)
            {
                _store[k] = d;
            }

            LoadAllData();
        }
    }
}
