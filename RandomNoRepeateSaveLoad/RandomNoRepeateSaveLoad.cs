using System.Collections.Generic;
using UnityEngine;

namespace TCW
{
    public class RandomNoRepeateSaveLoad
    {
        public int count = -1;
        private readonly List<int> _available = new List<int>();
        private SaveData _saveData = new SaveData();
        
        private string _dataKey;
        private string _countKey;
        
        public RandomNoRepeateSaveLoad SetCount(int value, string dataKey, string countKey)
        {
            count = value;
            
            if (PlayerPrefs.HasKey(countKey))
                count = PlayerPrefs.GetInt(countKey);
            else
                PlayerPrefs.SetInt(countKey, count);
            
            this._dataKey = dataKey;
            
            Init();
            
            return this;
        }
    
        public int GetAvailable()
        {
            if (_available.Count == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _available.Add(i);
                }
            }
    
            int randomIndex = Random.Range(0, _available.Count);
            int valueByIndex = _available[randomIndex];
            _available.RemoveAt(randomIndex);
    
            SaveIndMas();
    
            return valueByIndex;
        }
    
        public void Init()
        {
            if (PlayerPrefs.HasKey(_dataKey))
            {
                _available.Clear();
                _saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(_dataKey));
                
                for (int i = 0; i < _saveData.levelIndexes.Length; i++)
                {
                    _available.Add(_saveData.levelIndexes[i]);
                }
                
                return;
            }
            
            _saveData.levelIndexes = new int[count];
            _available.Clear();
            
            for (var i = 0; i < count; i++)
            {
                _available.Add(i);
                _saveData.levelIndexes[i] = _available[i];
            }
            
            PlayerPrefs.SetString(_dataKey, JsonUtility.ToJson(_saveData));
        }
    
        private void SaveIndMas()
        {
            _saveData.levelIndexes = new int[_available.Count];
            
            for (var i = 0; i < _available.Count; i++)
                _saveData.levelIndexes[i] = _available[i];
            
            PlayerPrefs.SetString(_dataKey, JsonUtility.ToJson(_saveData));
        }
    }
    
    [System.Serializable]
    public class SaveData
    {
        public int[] levelIndexes;
    }
}




