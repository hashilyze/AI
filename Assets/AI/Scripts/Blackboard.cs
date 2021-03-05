using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {
    public enum EParameterType { Bool, Int, Float, String, Vector2, Vector3, Object }


    [CreateAssetMenu]
    public class Blackboard : ScriptableObject, ISerializationCallbackReceiver {
        #region Serialized Data
        [SerializeField] private List<string> m_parameterNames = new List<string>();
        [SerializeField] private List<EParameterType> m_parameterTypes = new List<EParameterType>();

        [SerializeField] private List<bool> m_boolList = new List<bool>();
        [SerializeField] private List<int> m_intList = new List<int>();
        [SerializeField] private List<float> m_floatList = new List<float>();
        [SerializeField] private List<string> m_stringList = new List<string>();
        [SerializeField] private List<Vector2> m_vector2List = new List<Vector2>();
        [SerializeField] private List<Vector3> m_vector3List = new List<Vector3>();
        [SerializeField] private List<Object> m_objectList = new List<Object>();
        #endregion

        #region Runtime Data
        private readonly Dictionary<string, bool> m_boolDict = new Dictionary<string, bool>();
        private readonly Dictionary<string, int> m_intDict = new Dictionary<string, int>();
        private readonly Dictionary<string, float> m_floatDict = new Dictionary<string, float>();
        private readonly Dictionary<string, string> m_stringDict = new Dictionary<string, string>();
        private readonly Dictionary<string, Vector2> m_vector2Dict = new Dictionary<string, Vector2>();
        private readonly Dictionary<string, Vector3> m_vector3Dict = new Dictionary<string, Vector3>();
        private readonly Dictionary<string, Object> m_objectDict = new Dictionary<string, Object>();
        #endregion

        #region Data Table Access
        private IEnumerable<IList> GetSerializedTables () {
            yield return m_boolList;
            yield return m_intList;
            yield return m_floatList;
            yield return m_stringList;
            yield return m_vector2List;
            yield return m_vector3List;
            yield return m_objectList;
        }
        private IEnumerable<IDictionary> GetRuntimeTables () {
            yield return m_boolDict;
            yield return m_intDict;
            yield return m_floatDict;
            yield return m_stringDict;
            yield return m_vector2Dict;
            yield return m_vector3Dict;
            yield return m_objectDict;
        }

        private IList GetSerializedTable (EParameterType type) {
            switch (type) {
            case EParameterType.Bool: return m_boolList;
            case EParameterType.Int: return m_intList;
            case EParameterType.Float: return m_floatList;
            case EParameterType.String: return m_stringList;
            case EParameterType.Vector2: return m_vector2List;
            case EParameterType.Vector3: return m_vector3List;
            case EParameterType.Object: return m_objectList;
            default: throw new System.NotImplementedException();
            }
        }
        private IDictionary GetRuntimeTable (EParameterType type) {
            switch (type) {
            case EParameterType.Bool: return m_boolDict;
            case EParameterType.Int: return m_intDict;
            case EParameterType.Float: return m_floatDict;
            case EParameterType.String: return m_stringDict;
            case EParameterType.Vector2: return m_vector2Dict;
            case EParameterType.Vector3: return m_vector3Dict;
            case EParameterType.Object: return m_objectDict;
            default: throw new System.NotImplementedException();
            }
        }
        #endregion

        #region Serialize & Deserialize
        public void OnBeforeSerialize () {
            foreach (var values in GetSerializedTables()) {
                values.Clear();
            }

            for (int beg = 0, end = m_parameterNames.Count; beg != end; ++beg) {
                string name = m_parameterNames[beg];
                switch (m_parameterTypes[beg]) {
                case EParameterType.Bool: m_boolList.Add(m_boolDict[name]); break;
                case EParameterType.Int: m_intList.Add(m_intDict[name]); break;
                case EParameterType.Float: m_floatList.Add(m_floatDict[name]); break;
                case EParameterType.String: m_stringList.Add(m_stringDict[name]); break;
                case EParameterType.Vector2: m_vector2List.Add(m_vector2Dict[name]); break;
                case EParameterType.Vector3: m_vector3List.Add(m_vector3Dict[name]); break;
                case EParameterType.Object: m_objectList.Add(m_objectDict[name]); break;
                }
            }
        }
        public void OnAfterDeserialize () {
            foreach (var dict in GetRuntimeTables()) {
                dict.Clear();
            }

            int boolCursor = 0;
            int intCursor = 0;
            int floatCursor = 0;
            int stringCursor = 0;
            int vector2Cursor = 0;
            int vector3Cursor = 0;
            int objectCursor = 0;

            for (int beg = 0, end = m_parameterNames.Count; beg != end; ++beg) {
                string name = m_parameterNames[beg];
                switch (m_parameterTypes[beg]) {
                case EParameterType.Bool: m_boolDict.Add(name, m_boolList[boolCursor++]); break;
                case EParameterType.Int: m_intDict.Add(name, m_intList[intCursor++]); break;
                case EParameterType.Float: m_floatDict.Add(name, m_floatList[floatCursor++]); break;
                case EParameterType.String: m_stringDict.Add(name, m_stringList[stringCursor++]); break;
                case EParameterType.Vector2: m_vector2Dict.Add(name, m_vector2List[vector2Cursor++]); break;
                case EParameterType.Vector3: m_vector3Dict.Add(name, m_vector3List[vector3Cursor++]); break;
                case EParameterType.Object: m_objectDict.Add(name, m_objectList[objectCursor++]); break;
                }
            }
        }
        #endregion

        #region Interface: Get Value (Read)
        public bool GetBool (string key) => m_boolDict[key];
        public int GetInt (string key) => m_intDict[key];
        public float GetFloat (string key) => m_floatDict[key];
        public string GetString (string key) => m_stringDict[key];
        public Vector2 GetVector2 (string key) => m_vector2Dict[key];
        public Vector3 GetVector3 (string key) => m_vector3Dict[key];
        public Object GetObject (string key) => m_objectDict[key];
        #endregion

        #region Interface: Get Value All (Read)
        public IEnumerable<bool> GetBoolAll () => m_boolDict.Values;
        public IEnumerable<int> GetIntAll () => m_intDict.Values;
        public IEnumerable<float> GetFloatAll () => m_floatDict.Values;
        public IEnumerable<string> GetStringAll () => m_stringDict.Values;
        public IEnumerable<Vector2> GetVector2All () => m_vector2Dict.Values;
        public IEnumerable<Vector3> GetVector3All () => m_vector3Dict.Values;
        public IEnumerable<Object> GetObjectAll () => m_objectDict.Values;
        #endregion

        #region Interface: Set Value (Write)
        public void SetBool (string key, bool value) => SetValue(key, value, EParameterType.Bool, m_boolDict);
        public void SetInt (string key, int value) => SetValue(key, value, EParameterType.Int, m_intDict);
        public void SetFloat (string key, float value) => SetValue(key, value, EParameterType.Float, m_floatDict);
        public void SetString (string key, string value) => SetValue(key, value, EParameterType.String, m_stringDict);
        public void SetVector2 (string key, Vector2 value) => SetValue(key, value, EParameterType.Vector2, m_vector2Dict);
        public void SetVector3 (string key, Vector3 value) => SetValue(key, value, EParameterType.Vector3, m_vector3Dict);
        public void SetObject (string key, Object value) => SetValue(key, value, EParameterType.Object, m_objectDict);
        
        private void SetValue<T>(string key, T value, EParameterType type, Dictionary<string, T> dict) {
            int index = m_parameterNames.IndexOf(key);
            if(index < 0) { // Add new item
                m_parameterNames.Add(key);
                m_parameterTypes.Add(type);
            } else {
                EParameterType curType = m_parameterTypes[index];
                if(type != curType) { // Change item type
                    m_parameterTypes[index] = type;
                    switch (curType) { // Remove from previous table
                    case EParameterType.Bool: m_boolDict.Remove(name); break;
                    case EParameterType.Int: m_intDict.Remove(name); break;
                    case EParameterType.Float: m_floatDict.Remove(name); break;
                    case EParameterType.String: m_stringDict.Remove(name); break;
                    case EParameterType.Vector2: m_vector2Dict.Remove(name); break;
                    case EParameterType.Vector3: m_vector3Dict.Remove(name); break;
                    case EParameterType.Object: m_objectDict.Remove(name); break;
                    }
                }
            }
            dict[key] = value;
        }
        #endregion

        #region Interface: Remove (Write)
        public bool Remove(string key) {
            int index = m_parameterNames.IndexOf(key);
            // No exist item with key
            if (index < 0) return false;

            switch (m_parameterTypes[index]) {
            case EParameterType.Bool: m_boolDict.Remove(name); break;
            case EParameterType.Int: m_intDict.Remove(name); break;
            case EParameterType.Float: m_floatDict.Remove(name); break;
            case EParameterType.String: m_stringDict.Remove(name); break;
            case EParameterType.Vector2: m_vector2Dict.Remove(name); break;
            case EParameterType.Vector3: m_vector3Dict.Remove(name); break;
            case EParameterType.Object: m_objectDict.Remove(name); break;
            }

            m_parameterNames.RemoveAt(index);
            m_parameterTypes.RemoveAt(index);
            return true;
        }
        #endregion

        #region Interface: Parameter Infos
        public IEnumerable<string> GetParameterNames () => m_parameterNames;
        public IEnumerable<EParameterType> GetParameterTypes () => m_parameterTypes;
        public bool ExistKey (string key) => m_parameterNames.Contains(key);
        #endregion
    }
}