using System;
using UnityEngine;

namespace CyberneticStudios.SOFramework
{
    public abstract class Variable<T> : ScriptableObject, ISerializationCallbackReceiver
    {

#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif

        public System.Action<T> OnChanged;

        [SerializeField] private T _initialValue;
        [SerializeField] private T _value;

        private T _prevValue;

        public T value
        {
            get => _value;
            set
            {
                this._prevValue = this._value;
                this._value = value;
                if (!this._prevValue.Equals(this._value)) OnChanged?.Invoke(value);
            }
        }

        public void ResetVariable()
        {
            // _value = default(T);
            value = _initialValue;
        }

        public void OnAfterDeserialize()
        {
            OnChanged?.Invoke(_value);
        }

        public void OnBeforeSerialize() { }
    }
}