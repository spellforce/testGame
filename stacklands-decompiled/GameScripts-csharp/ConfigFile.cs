using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

public class ConfigFile
{
	public Mod Mod;

	public JObject Data;

	public Action OnSave;

	public List<ConfigEntryBase> Entries = new List<ConfigEntryBase>();

	public ConfigFile(Mod mod, string jsonPath)
	{
		Mod = mod;
		string text = File.ReadAllText(jsonPath);
		if (string.IsNullOrEmpty(text))
		{
			text = "{}";
		}
		Data = JObject.Parse(text);
	}

	public ConfigEntry<T> GetEntry<T>(string property, object defaultValue = null, ConfigUI ui = null)
	{
		return ((ConfigEntry<T>)Entries.FirstOrDefault((ConfigEntryBase e) => e.Name == property)) ?? new ConfigEntry<T>(property, this, defaultValue, ui);
	}

	public T GetValue<T>(string property)
	{
		JToken val = default(JToken);
		if (Data.TryGetValue(property, ref val))
		{
			return val.ToObject<T>();
		}
		return default(T);
	}

	public object GetValue(string property, Type valueType)
	{
		JToken val = default(JToken);
		if (Data.TryGetValue(property, ref val))
		{
			return val.ToObject(valueType);
		}
		return null;
	}

	internal void SetValue(string property, object data)
	{
		Data[property] = (JToken)((data == null) ? ((object)JValue.CreateNull()) : ((object)JToken.FromObject(data)));
	}

	public void Save()
	{
		File.WriteAllText(Path.Combine(Mod.Path, "config.json"), ((object)Data).ToString());
		OnSave?.Invoke();
	}
}
