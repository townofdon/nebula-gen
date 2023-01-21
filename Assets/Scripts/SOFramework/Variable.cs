using System;
using UnityEngine;

namespace CyberneticStudios.SOFramework
{
    public abstract class Variable<T> : ScriptableObject
    {

#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif

        public System.Action<T> OnChanged;

        [SerializeField] private T _initialValue;
        [SerializeField] private T _value;

        public T value
        {
            get => _value;
            set
            {
                this._value = value;

                OnChanged?.Invoke(value);
            }
        }

        public void ResetVariable()
        {
            // _value = default(T);
            _value = _initialValue;
        }
    }
}