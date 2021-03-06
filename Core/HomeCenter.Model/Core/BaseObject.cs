﻿using HomeCenter.Broker;
using HomeCenter.Model.Messages;
using HomeCenter.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.Model.Core
{
    public class BaseObject : IPropertySource, IBaseObject
    {
        [Map] private Dictionary<string, string> _properties { get; set; } = new Dictionary<string, string>();

        public string Uid
        {
            get => AsString(MessageProperties.Uid, GetType().Name);
            set
            {
                // Prevent for setting UID null during initialization
                if (value != null)
                {
                    SetProperty(MessageProperties.Uid, value);
                }
            }
        }

        public string Type
        {
            get => AsString(MessageProperties.Type);
            set => SetProperty(MessageProperties.Type, value);
        }

        public BaseObject()
        {
            Type = GetType().Name;
        }

        public BaseObject SetProperties(BaseObject source)
        {
            foreach (var property in source.GetProperties())
            {
                SetProperty(property.Key, property.Value);
            }
            return this;
        }

        public override string ToString() => GetProperties()?.ToFormatedString();

        public bool ContainsProperty(string propertyName) => _properties.ContainsKey(propertyName);

        public BaseObject SetProperty(string propertyName, string value)
        {
            _properties[propertyName] = value;
            return this;
        }

        public BaseObject SetProperty(string propertyName, DateTimeOffset value)
        {
            _properties[propertyName] = value.ToString();
            return this;
        }

        public BaseObject SetProperty(string propertyName, TimeSpan value)
        {
            _properties[propertyName] = value.ToString();
            return this;
        }

        public BaseObject SetProperty(string propertyName, int value)
        {
            _properties[propertyName] = value.ToString();
            return this;
        }

        public BaseObject SetProperty(string propertyName, double value)
        {
            _properties[propertyName] = value.ToString();
            return this;
        }

        public BaseObject SetProperty(string propertyName, bool value)
        {
            _properties[propertyName] = value.ToString();
            return this;
        }

        public BaseObject SetProperty(string propertyName, byte[] value)
        {
            _properties[propertyName] = value.ToHex();
            return this;
        }

        public BaseObject SetProperty(string propertyName, IDictionary<string, string> value)
        {
            _properties[propertyName] = JsonConvert.SerializeObject(value);
            return this;
        }

        public BaseObject SetPropertyList(string propertyName, params string[] values)
        {
            _properties[propertyName] = string.Join(", ", values);
            return this;
        }

        public IReadOnlyDictionary<string, string> GetProperties()
        {
            return _properties.AsReadOnly();
        }

        public IEnumerable<string> GetPropetiesKeys() => _properties.Keys;

        public string this[string propertyName]
        {
            get
            {
                if (!ContainsProperty(propertyName)) throw new KeyNotFoundException($"Property {propertyName} not found on component {Uid}");
                return _properties[propertyName];
            }
            set { SetProperty(propertyName, value); }
        }

        public void SetEmptyProperty(string propertyName)
        {
            if (_properties.ContainsKey(propertyName)) return;
            _properties[propertyName] = string.Empty;
        }

        public bool AsBool(string propertyName, bool? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (bool.TryParse(_properties[propertyName], out bool value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper bool value");
        }

        public int AsInt(string propertyName, int? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (int.TryParse(_properties[propertyName], out int value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper int value");
        }

        public byte AsByte(string propertyName, byte? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (byte.TryParse(_properties[propertyName], out byte value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper byte value");
        }

        public DateTime AsDate(string propertyName, DateTime? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (DateTime.TryParse(_properties[propertyName], out DateTime value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper date value");
        }

        public TimeSpan AsTime(string propertyName, TimeSpan? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (TimeSpan.TryParse(_properties[propertyName], out TimeSpan value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper time value");
        }

        public TimeSpan AsIntTime(string propertyName, int? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                if (defaultValue.HasValue)
                {
                    return TimeSpan.FromMilliseconds(defaultValue.Value);
                }
                throw new ArgumentException(propertyName);
            }

            if (int.TryParse(_properties[propertyName], out int value))
            {
                return TimeSpan.FromMilliseconds(value);
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper time value");
        }

        public double AsDouble(string propertyName, double defaultValue = double.MinValue)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                if (defaultValue != double.MinValue) return defaultValue;
                throw new ArgumentException(propertyName);
            }
            return AsDoubleInner(propertyName);
        }

        public double? AsNullableDouble(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName)) return null;

            return AsDoubleInner(propertyName);
        }

        private double AsDoubleInner(string propertyName)
        {
            if (_properties[propertyName].ParseAsDouble(out double value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper double value");
        }

        public uint AsUint(string propertyName, uint? defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            if (uint.TryParse(_properties[propertyName], out uint value))
            {
                return value;
            }

            throw new FormatException($"Property {propertyName} value {_properties[propertyName]} is not proper uint value");
        }

        public string AsString(string propertyName, string defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            return _properties[propertyName];
        }

        public IList<string> AsList(string propertyName, IEnumerable<string> defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return (IList<string>)defaultValue ?? throw new ArgumentException(propertyName);

            return _properties[propertyName].Split(',').Select(x => x.Trim()).ToList();
        }

        public byte[] AsByteArray(string propertyName, byte[] defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            return _properties[propertyName].ToBytes();
        }

        public IDictionary<string, string> AsDictionary(string propertyName, IDictionary<string, string> defaultValue = null)
        {
            if (!_properties.ContainsKey(propertyName)) return defaultValue ?? throw new ArgumentException(propertyName);

            return JsonConvert.DeserializeObject<IDictionary<string, string>>(_properties[propertyName]);
        }
    }
}