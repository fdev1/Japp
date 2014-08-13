using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Japp
{
	/// <summary>
	/// Holds a device configuration bit.
	/// </summary>
	public class ConfigurationBit : ConfigurationElement
	{
		/// <summary>
		/// Address.
		/// </summary>
		[ConfigurationProperty("address", IsRequired=true)]
		public uint Address
		{
			get
			{
				return (uint)this["address"];
			}
			set
			{
				this["address"] = value;
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[ConfigurationProperty("name", IsRequired=true)]
		public string Namme
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		/// <summary>
		/// Mask.
		/// </summary>
		[ConfigurationProperty("mask", IsRequired=true)]
		public byte Mask
		{
			get
			{
				return (byte)this["mask"];
			}
			set
			{
				this["mask"] = value;
			}
		}

		/// <summary>
		/// Ones mask.
		/// </summary>
		[ConfigurationProperty("onesMask", IsRequired=false, DefaultValue=(byte)0x00)]
		public byte OnesMask
		{
			get
			{
				return (byte)this["onesMask"];
			}
			set
			{
				this["onesMask"] = value;
			}
		}

	}

    /// <summary>
    /// Represents a device configuration.
    /// </summary>
    public class Device : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// Gets the value of the DEVID register.
        /// </summary>
        [ConfigurationProperty("DEVID", IsRequired=true)]
        public string DEVID
        {
            get
            {
                return this["DEVID"].ToString();
            }
            set
            {
                this["DEVID"] = value;
            }
        }

        /// <summary>
        /// Gets the value of the DEVREV register
        /// </summary>
        [ConfigurationProperty("DEVREV", IsRequired=true)]
        public string DEVREV
        {
            get
            {
                return this["DEVREV"].ToString();
            }
            set
            {
                this["DEVREV"] = value;
            }
        }

        /// <summary>
        /// Gets the program memory size of the device.
        /// </summary>
        [ConfigurationProperty("programMemorySize", IsRequired=true)]
        public uint ProgramMemroySize
        {
            get
            {
                return (uint) this["programMemorySize"];
            }
            set
            {
                this["programMemorySize"] = value;
            }
        }

		/// <summary>
		/// Gets or sets the address of the configuration bits.
		/// </summary>
		[ConfigurationProperty("konfigAddress", IsRequired=true, DefaultValue=0x0U)]
		public uint ConfigAddress
		{
			get
			{
				return (uint)this["konfigAddress"];
			}
			set
			{
				this["konfigAddress"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of configuration bytes
		/// </summary>
		[ConfigurationProperty("konfigBytes", IsRequired = false, DefaultValue = 0x0U)]
		public uint ConfigBytes
		{
			get
			{
				return (uint)this["konfigBytes"];
			}
			set
			{
				this["konfigBytes"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the family that owns this device.
		/// </summary>
		public DeviceFamily Family
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the device's configuration bits.
		/// </summary>
		[ConfigurationProperty("konfigurationBits")]
		public ConfigurationBitCollection ConfigurationBits
		{
			get
			{
				return (ConfigurationBitCollection)this["konfigurationBits"];
			}
			set
			{
				this["konfigurationBits"] = value;
			}
		}

		/// <summary>
		/// Represents the collection of configuration bits
		/// </summary>
		public class ConfigurationBitCollection : ConfigurationElementCollection
		{
			/// <summary>
			/// Creates a new element.
			/// </summary>
			/// <returns>The element.</returns>
			protected override ConfigurationElement CreateNewElement()
			{
				return new ConfigurationBit();
			}

			/// <summary>
			/// Get the element key.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <returns>The key.</returns>
			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((ConfigurationBit)element).Address;
			}

			/// <summary>
			/// Gets the collection type.
			/// </summary>
			public override ConfigurationElementCollectionType CollectionType
			{
				get
				{
					return ConfigurationElementCollectionType.BasicMap;
				}
			}

			/// <summary>
			/// Gets the element name.
			/// </summary>
			protected override string ElementName
			{
				get
				{
					return "konfigurationBit";
				}
			}
		}
	}

    /// <summary>
    /// Represents the configuration settings for a device family
    /// </summary>
    public class DeviceFamily : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of the device family.
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of devices in this family.
        /// </summary>
        [ConfigurationProperty("devices")]
        public DevicesCollection Devices
        {
            get
            {
                DevicesCollection col = (DevicesCollection)this["devices"];
				return col;
            }
            set
            {
                this["devices"] = value;
            }
        }

        /// <summary>
        /// Gets the type of the class used to program this device family.
        /// </summary>
        [ConfigurationProperty("programmerClass")]
        public string ProgrammerClass
        {
            get
            {
                return (string) this["programmerClass"];
            }
            set
            {
                this["programmerClass"] = value;
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether the device has
		/// volatile configuration bits.
		/// </summary>
		[ConfigurationProperty("volatileConfig", IsRequired=false, DefaultValue=false)]
		public bool IsVolatileConfig
		{
			get
			{
				return (bool)this["volatileConfig"];
			}
			set
			{
				this["volatileConfig"] = value;
			}
		}

		public void UpdateDevices()
		{
			foreach (Device dev in this.Devices)
			{
				dev.Family = this;
			}
		}

		/// <summary>
		/// Represents the collection of devices within a family.
		/// </summary>
		public class DevicesCollection : ConfigurationElementCollection
		{
			/// <summary>
			/// Creates a new element.
			/// </summary>
			/// <returns>The element.</returns>
			protected override ConfigurationElement CreateNewElement()
			{
				Device dev = new Device();
				return dev;
			}

			/// <summary>
			/// Get the element key.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <returns>The key.</returns>
			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((Device)element).Name;
			}

			/// <summary>
			/// Gets the collection type.
			/// </summary>
			public override ConfigurationElementCollectionType CollectionType
			{
				get
				{
					return ConfigurationElementCollectionType.BasicMap;
				}
			}

			/// <summary>
			/// Gets the element name.
			/// </summary>
			protected override string ElementName
			{
				get
				{
					return "device";
				}
			}
		}
	}

	/// <summary>
	/// Device configuration handler
	/// </summary>
    public class DeviceConfig : ConfigurationSection 
    {
		/// <summary>
		/// Gets or sets the collection of device families
		/// </summary>
		[ConfigurationProperty("families")]
		public DeviceFamiliesCollection Families
		{
			get
			{
				return (DeviceFamiliesCollection)this["families"];
			}
			set
			{
				this["families"] = value;
			}
		}

		public void UpdateFamilies()
		{
			foreach (DeviceFamily fam in this.Families)
			{
				fam.UpdateDevices();
			}
		}

		/// <summary>
		/// Represents the collection of device family settings.
		/// </summary>
		public class DeviceFamiliesCollection : ConfigurationElementCollection
		{
			/// <summary>
			/// Creates a new element.
			/// </summary>
			/// <returns>The element.</returns>
			protected override ConfigurationElement CreateNewElement()
			{
				return new DeviceFamily();
			}

			/// <summary>
			/// Gets the element key.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <returns>The key.</returns>
			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((DeviceFamily)element).Name;
			}

			/// <summary>
			/// Gets the collectin type.
			/// </summary>
			public override ConfigurationElementCollectionType CollectionType
			{
				get
				{
					return ConfigurationElementCollectionType.BasicMap;
				}
			}

			/// <summary>
			/// Gets the element name.
			/// </summary>
			protected override string ElementName
			{
				get
				{
					return "family";
				}
			}
		}
    }
}
