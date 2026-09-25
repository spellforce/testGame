using System;
using Newtonsoft.Json;
using UnityEngine;

internal class StringColorConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Color);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		if (ColorUtility.TryParseHtmlString((string)reader.Value, ref val))
		{
			return val;
		}
		Debug.LogWarning((object)$"Failed to parse color \"{reader.Value}\"");
		return Color.black;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		writer.WriteValue(ColorUtility.ToHtmlStringRGBA((Color)value));
	}
}
