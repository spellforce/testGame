using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class SchemaGenerator
{
	private static JObject EnumDefs = new JObject();

	public static JObject CardBaseProps;

	public static JObject BlueprintBaseProps;

	public static List<string> PropBlacklist;

	public static Dictionary<string, JObject> PropOverride;

	public static Dictionary<Type, JObject> TypeLookup;

	public static JObject Defs;

	public static JObject TypeString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("string"));
		return val;
	}

	public static JObject TypeString(string description)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("string"));
		val.Add("description", JToken.op_Implicit(description));
		return val;
	}

	public static JObject TypeInt()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("integer"));
		return val;
	}

	public static JObject TypeFloat()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("number"));
		return val;
	}

	public static JObject TypeBool()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("boolean"));
		return val;
	}

	public static JObject TypeColor()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("string"));
		val.Add("pattern", JToken.op_Implicit("^((#[0-9A-Fa-f]{8})|(#[0-9A-Fa-f]{6})|(#[0-9A-Fa-f]{4})|(#[0-9A-Fa-f]{3}))$"));
		return val;
	}

	public static void GenerateSchemas(string path)
	{
		if (!Directory.Exists(path))
		{
			throw new Exception("'" + path + "' is not a valid folder!");
		}
		GenerateCardSchema(Path.Combine(path, "card.schema.json"));
		GenerateBlueprintSchema(Path.Combine(path, "blueprint.schema.json"));
		GenerateBoosterSchema(Path.Combine(path, "boosterpack.schema.json"));
	}

	public static void GenerateCardSchema(string path)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_0143: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0168: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_017a: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected O, but got Unknown
		Debug.Log((object)"Generating card schema..");
		JObject baseSchema = GetBaseSchema();
		JToken obj = baseSchema["allOf"];
		JArray val = (JArray)(object)((obj is JArray) ? obj : null);
		val.Add((JToken)(object)CardBaseProps);
		val.Add((JToken)(object)OneOfAny("nameTerm", "nameOverride"));
		val.Add((JToken)(object)OneOfAny("descriptionTerm", "descriptionOverride"));
		JObject val2 = new JObject();
		val.Add((JToken)(object)val2);
		JArray val3 = (JArray)(object)(val2["anyOf"] = (JToken)new JArray());
		foreach (Type value in ModManager.CardClasses.Values)
		{
			if (typeof(Blueprint).IsAssignableFrom(value))
			{
				continue;
			}
			Type type = value;
			List<Type> list = new List<Type> { value };
			while (type != typeof(CardData))
			{
				type = type.BaseType;
				list.Add(type);
			}
			JObject val5 = new JObject();
			JObject val6 = new JObject();
			JObject val7 = new JObject();
			JObject val8 = new JObject();
			val8.Add("const", JToken.op_Implicit(value.ToString()));
			val7.Add("script", (JToken)val8);
			val6.Add("properties", (JToken)val7);
			JArray val9 = new JArray();
			val9.Add(JToken.op_Implicit("script"));
			val6.Add("required", (JToken)val9);
			val5.Add("if", (JToken)val6);
			val5.Add("then", (JToken)new JObject());
			JObject val10 = val5;
			val3.Add((JToken)(object)val10);
			JObject val11 = new JObject();
			val10["then"][(object)"properties"] = (JToken)(object)val11;
			foreach (Type item in list)
			{
				FieldInfo[] fields = item.GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					if (FieldToJson(fieldInfo, out var obj2))
					{
						val11["_" + fieldInfo.Name] = (JToken)(object)obj2;
					}
				}
			}
			JArray val12 = new JArray();
			val12.Add(JToken.op_Implicit("script"));
			val10["required"] = (JToken)val12;
		}
		val3.Add((JToken)new JObject());
		baseSchema["$defs"] = (JToken)(object)Defs;
		baseSchema["$defs"][(object)"enum"] = (JToken)(object)EnumDefs;
		File.WriteAllText(path, ((object)baseSchema).ToString());
		Debug.Log((object)"Done!");
	}

	public static void GenerateBlueprintSchema(string path)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0122: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0147: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0159: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		Debug.Log((object)"Generating blueprint schema..");
		JObject baseSchema = GetBaseSchema();
		JToken obj = baseSchema["allOf"];
		JArray val = (JArray)(object)((obj is JArray) ? obj : null);
		val.Add((JToken)(object)BlueprintBaseProps);
		val.Add((JToken)(object)OneOfAny("nameTerm", "nameOverride"));
		JObject val2 = new JObject();
		val.Add((JToken)(object)val2);
		JArray val3 = (JArray)(object)(val2["anyOf"] = (JToken)new JArray());
		foreach (Type value in ModManager.CardClasses.Values)
		{
			if (!typeof(Blueprint).IsAssignableFrom(value))
			{
				continue;
			}
			Type type = value;
			List<Type> list = new List<Type> { value };
			while (type != typeof(CardData))
			{
				type = type.BaseType;
				list.Add(type);
			}
			JObject val5 = new JObject();
			JObject val6 = new JObject();
			JObject val7 = new JObject();
			JObject val8 = new JObject();
			val8.Add("const", JToken.op_Implicit(value.ToString()));
			val7.Add("script", (JToken)val8);
			val6.Add("properties", (JToken)val7);
			JArray val9 = new JArray();
			val9.Add(JToken.op_Implicit("script"));
			val6.Add("required", (JToken)val9);
			val5.Add("if", (JToken)val6);
			val5.Add("then", (JToken)new JObject());
			JObject val10 = val5;
			val3.Add((JToken)(object)val10);
			JObject val11 = new JObject();
			val10["then"][(object)"properties"] = (JToken)(object)val11;
			foreach (Type item in list)
			{
				FieldInfo[] fields = item.GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					if (FieldToJson(fieldInfo, out var obj2))
					{
						val11["_" + fieldInfo.Name] = (JToken)(object)obj2;
					}
				}
			}
			JArray val12 = new JArray();
			val12.Add(JToken.op_Implicit("script"));
			val10["required"] = (JToken)val12;
		}
		val3.Add((JToken)new JObject());
		baseSchema["$defs"] = (JToken)(object)Defs;
		baseSchema["$defs"][(object)"enum"] = (JToken)(object)EnumDefs;
		File.WriteAllText(path, ((object)baseSchema).ToString());
		Debug.Log((object)"Done!");
	}

	public static void GenerateBoosterSchema(string path)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0119: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_013e: Expected O, but got Unknown
		Debug.Log((object)"Generating boosterpack schema..");
		JObject baseSchema = GetBaseSchema();
		JToken obj = baseSchema["allOf"];
		JToken obj2 = ((obj is JArray) ? obj : null);
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("object"));
		JObject val2 = new JObject();
		val2.Add("$schema", (JToken)(object)TypeString());
		val2.Add("id", (JToken)(object)TypeString());
		val2.Add("nameTerm", (JToken)(object)Ref("term"));
		val2.Add("nameOverride", (JToken)(object)TypeString());
		val2.Add("minQuestCount", (JToken)(object)TypeInt());
		val2.Add("cost", (JToken)(object)TypeInt());
		val2.Add("icon", (JToken)(object)TypeString("Sprite. Value must be the file name of an image in your mods Icons/ folder."));
		val2.Add("location", (JToken)(object)NamesFromEnum(typeof(Location)));
		JObject val3 = new JObject();
		val3.Add("type", JToken.op_Implicit("array"));
		val3.Add("items", (JToken)(object)Ref("typeCardBag"));
		val2.Add("cardBags", (JToken)val3);
		val.Add("properties", (JToken)val2);
		JArray val4 = new JArray();
		val4.Add(JToken.op_Implicit("id"));
		val.Add("required", (JToken)val4);
		((JArray)obj2).Add((JToken)val);
		((JArray)obj2).Add((JToken)(object)OneOfAny("nameTerm", "nameOverride"));
		baseSchema["$defs"] = (JToken)(object)Defs;
		baseSchema["$defs"][(object)"enum"] = (JToken)(object)EnumDefs;
		File.WriteAllText(path, ((object)baseSchema).ToString());
		Debug.Log((object)"Done!");
	}

	public static bool FieldToJson(FieldInfo field, out JObject obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		obj = new JObject();
		string text = $"{field.DeclaringType}.{field.Name}";
		if (PropOverride.TryGetValue(text, out var value))
		{
			obj = value;
			return true;
		}
		if (PropBlacklist.Contains(text))
		{
			return false;
		}
		if (field.FieldType.IsEnum)
		{
			obj = NamesFromEnum(field.FieldType);
			return true;
		}
		if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && TypeLookup.TryGetValue(field.FieldType.GetGenericArguments()[0], out var value2))
		{
			JObject val = new JObject();
			val.Add("type", JToken.op_Implicit("array"));
			val.Add("items", (JToken)(object)value2);
			obj = val;
			return true;
		}
		if (field.FieldType.IsArray && TypeLookup.TryGetValue(field.FieldType.GetElementType(), out var value3))
		{
			JObject val2 = new JObject();
			val2.Add("type", JToken.op_Implicit("array"));
			val2.Add("items", (JToken)(object)value3);
			obj = val2;
			return true;
		}
		if (TypeLookup.TryGetValue(field.FieldType, out var value4))
		{
			obj = value4;
			return true;
		}
		Debug.LogWarning((object)$"FieldToJson matched nothing for {field.DeclaringType}.{field.Name} ({field.FieldType})");
		return false;
	}

	public static JObject NamesFromEnum(Type enumType)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		if (EnumDefs[enumType.ToString()] == null)
		{
			List<string> list = new List<string>();
			foreach (string name in EnumHelper.GetNames(enumType))
			{
				list.Add(name);
			}
			EnumDefs[enumType.ToString()] = (JToken)new JArray((object)list);
		}
		return Ref("enum/" + enumType.ToString());
	}

	public static JObject GetCombatStats()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("object"));
		val.Add("properties", (JToken)new JObject());
		JObject val2 = val;
		JToken obj = val2["properties"];
		JObject val3 = (JObject)(object)((obj is JObject) ? obj : null);
		FieldInfo[] fields = typeof(CombatStats).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (TypeLookup.TryGetValue(fieldInfo.FieldType, out var value))
			{
				val3[fieldInfo.Name] = (JToken)(object)value;
			}
		}
		return val2;
	}

	public static JObject GetSubprint()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("object"));
		val.Add("properties", (JToken)new JObject());
		JObject val2 = val;
		JToken obj = val2["properties"];
		JObject val3 = (JObject)(object)((obj is JObject) ? obj : null);
		FieldInfo[] fields = typeof(Subprint).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (FieldToJson(fieldInfo, out var obj2))
			{
				val3[fieldInfo.Name] = (JToken)(object)obj2;
			}
		}
		return val2;
	}

	public static JObject GetCardBag()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0050: Expected O, but got Unknown
		//IL_0055: Expected O, but got Unknown
		//IL_0056: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_015d: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0182: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0194: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("object"));
		JArray val2 = new JArray();
		JObject val3 = new JObject();
		JObject val4 = new JObject();
		val4.Add("CardBagType", (JToken)(object)TypeString());
		val3.Add("properties", (JToken)val4);
		val2.Add((JToken)val3);
		val.Add("allOf", (JToken)val2);
		JObject val5 = val;
		JToken obj = val5["allOf"];
		JArray val6 = (JArray)(object)((obj is JArray) ? obj : null);
		foreach (KeyValuePair<CardBagType, List<string>> item in new Dictionary<CardBagType, List<string>>
		{
			{
				CardBagType.Chances,
				new List<string> { "Chances" }
			},
			{
				CardBagType.SetPack,
				new List<string> { "SetPackCards" }
			},
			{
				CardBagType.SetCardBag,
				new List<string> { "SetCardBag", "UseFallbackBag", "FallbackBag" }
			},
			{
				CardBagType.Enemies,
				new List<string> { "EnemyCardBag", "StrengthLevel" }
			}
		})
		{
			CardBagType key = item.Key;
			List<string> value = item.Value;
			CardBagType cardBagType = key;
			JObject val7 = new JObject();
			JObject val8 = new JObject();
			JObject val9 = new JObject();
			JObject val10 = new JObject();
			val10.Add("const", JToken.op_Implicit(cardBagType.ToString()));
			val9.Add("CardBagType", (JToken)val10);
			val8.Add("properties", (JToken)val9);
			JArray val11 = new JArray();
			val11.Add(JToken.op_Implicit("CardBagType"));
			val8.Add("required", (JToken)val11);
			val7.Add("if", (JToken)val8);
			val7.Add("then", (JToken)new JObject());
			JObject val12 = val7;
			JObject val13 = new JObject();
			val12["then"][(object)"properties"] = (JToken)(object)val13;
			foreach (string item2 in value)
			{
				if (FieldToJson(typeof(CardBag).GetField(item2), out var obj2))
				{
					val13[item2] = (JToken)(object)obj2;
				}
			}
			val6.Add((JToken)(object)val12);
		}
		return val5;
	}

	public static JObject GetCardChance()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0080: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00c2: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e7: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011c: Expected O, but got Unknown
		//IL_0121: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_0174: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_0199: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01db: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_0200: Expected O, but got Unknown
		//IL_0205: Expected O, but got Unknown
		//IL_020a: Expected O, but got Unknown
		//IL_020f: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_023f: Expected O, but got Unknown
		//IL_0244: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_0286: Expected O, but got Unknown
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Expected O, but got Unknown
		//IL_02ab: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		//IL_02f5: Expected O, but got Unknown
		//IL_02fa: Expected O, but got Unknown
		//IL_02ff: Expected O, but got Unknown
		//IL_0300: Expected O, but got Unknown
		JObject val = new JObject();
		JArray val2 = new JArray();
		JObject val3 = new JObject();
		JObject val4 = new JObject();
		val4.Add("Id", (JToken)(object)Ref("cardId"));
		val4.Add("Chance", (JToken)(object)TypeInt());
		val4.Add("HasMaxCount", (JToken)(object)TypeBool());
		val4.Add("HasPrerequisiteCard", (JToken)(object)TypeBool());
		val4.Add("IsEnemy", (JToken)(object)TypeBool());
		val3.Add("properties", (JToken)val4);
		val2.Add((JToken)val3);
		JObject val5 = new JObject();
		JObject val6 = new JObject();
		JObject val7 = new JObject();
		JObject val8 = new JObject();
		val8.Add("const", JToken.op_Implicit(true));
		val7.Add("HasPrerequisiteCard", (JToken)val8);
		val6.Add("properties", (JToken)val7);
		JArray val9 = new JArray();
		val9.Add(JToken.op_Implicit("HasPrerequisiteCard"));
		val6.Add("required", (JToken)val9);
		val5.Add("if", (JToken)val6);
		JObject val10 = new JObject();
		JObject val11 = new JObject();
		val11.Add("PrerequisiteCardId", (JToken)(object)Ref("cardId"));
		val10.Add("properties", (JToken)val11);
		val5.Add("then", (JToken)val10);
		val2.Add((JToken)val5);
		JObject val12 = new JObject();
		JObject val13 = new JObject();
		JArray val14 = new JArray();
		JObject val15 = new JObject();
		JObject val16 = new JObject();
		JObject val17 = new JObject();
		val17.Add("const", JToken.op_Implicit(true));
		val16.Add("HasMaxCount", (JToken)val17);
		val15.Add("properties", (JToken)val16);
		JArray val18 = new JArray();
		val18.Add(JToken.op_Implicit("HasMaxCount"));
		val15.Add("required", (JToken)val18);
		val14.Add((JToken)val15);
		JObject val19 = new JObject();
		JObject val20 = new JObject();
		JObject val21 = new JObject();
		JObject val22 = new JObject();
		val22.Add("const", JToken.op_Implicit(true));
		val21.Add("IsEnemy", (JToken)val22);
		val20.Add("properties", (JToken)val21);
		JArray val23 = new JArray();
		val23.Add(JToken.op_Implicit("IsEnemy"));
		val20.Add("required", (JToken)val23);
		val19.Add("not", (JToken)val20);
		val14.Add((JToken)val19);
		val13.Add("allOf", (JToken)val14);
		val12.Add("if", (JToken)val13);
		JObject val24 = new JObject();
		JObject val25 = new JObject();
		val25.Add("MaxCountToGive", (JToken)(object)TypeInt());
		val24.Add("properties", (JToken)val25);
		val12.Add("then", (JToken)val24);
		val2.Add((JToken)val12);
		JObject val26 = new JObject();
		JObject val27 = new JObject();
		JObject val28 = new JObject();
		JObject val29 = new JObject();
		val29.Add("const", JToken.op_Implicit(true));
		val28.Add("IsEnemy", (JToken)val29);
		val27.Add("properties", (JToken)val28);
		JArray val30 = new JArray();
		val30.Add(JToken.op_Implicit("IsEnemy"));
		val27.Add("required", (JToken)val30);
		val26.Add("if", (JToken)val27);
		JObject val31 = new JObject();
		JObject val32 = new JObject();
		val32.Add("EnemyBag", (JToken)(object)NamesFromEnum(typeof(EnemySetCardBag)));
		val32.Add("Strength", (JToken)(object)TypeFloat());
		val31.Add("properties", (JToken)val32);
		val26.Add("then", (JToken)val31);
		val2.Add((JToken)val26);
		val.Add("allOf", (JToken)val2);
		return val;
	}

	public static JObject GetCardIds()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		List<string> list = WorldManager.instance.CardDataPrefabs.Select((CardData c) => c.Id).ToList();
		list.Remove("ideas_base");
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("string"));
		val.Add("enum", (JToken)new JArray((object)list));
		return val;
	}

	public static JObject GetTerms()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0064: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("string"));
		JArray val2 = new JArray();
		((JContainer)val2).Add((object)SokLoc.instance.CurrentLocSet.AllTerms.Select((SokTerm t) => t.Id));
		val.Add("enum", (JToken)val2);
		return val;
	}

	public static JObject GetBaseSchema()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c3: Expected O, but got Unknown
		string text = "Stacklands (v" + Application.version + ")";
		foreach (Mod loadedMod in ModManager.LoadedMods)
		{
			text = text + "; " + loadedMod.Manifest.Id + " (v" + loadedMod.Manifest.Version + ")";
		}
		JObject val = new JObject();
		val.Add("$comment", JToken.op_Implicit(text));
		val.Add("type", JToken.op_Implicit("object"));
		val.Add("allOf", (JToken)new JArray());
		return val;
	}

	public static JObject OneOfAny(params string[] props)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_0016: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0056: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("oneOf", (JToken)new JArray());
		JObject val2 = val;
		foreach (string text in props)
		{
			JToken obj = val2["oneOf"];
			JToken obj2 = ((obj is JArray) ? obj : null);
			JObject val3 = new JObject();
			JArray val4 = new JArray();
			val4.Add(JToken.op_Implicit(text));
			val3.Add("required", (JToken)val4);
			((JArray)obj2).Add((JToken)val3);
		}
		return val2;
	}

	public static JObject Ref(string str)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("$ref", JToken.op_Implicit("#/$defs/" + str));
		return val;
	}

	static SchemaGenerator()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0132: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Expected O, but got Unknown
		//IL_026a: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_028f: Expected O, but got Unknown
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Expected O, but got Unknown
		//IL_07ab: Expected O, but got Unknown
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Expected O, but got Unknown
		//IL_0800: Expected O, but got Unknown
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Expected O, but got Unknown
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("object"));
		JObject val2 = new JObject();
		val2.Add("$schema", (JToken)(object)TypeString());
		val2.Add("id", (JToken)(object)TypeString());
		val2.Add("nameTerm", (JToken)(object)Ref("term"));
		val2.Add("nameOverride", (JToken)(object)TypeString());
		val2.Add("descriptionTerm", (JToken)(object)Ref("term"));
		val2.Add("descriptionOverride", (JToken)(object)TypeString());
		val2.Add("type", (JToken)(object)NamesFromEnum(typeof(CardType)));
		val2.Add("icon", (JToken)(object)TypeString("Sprite. Value must be the file name of an image in your mods Icons/ folder."));
		val2.Add("pickupSound", (JToken)(object)TypeString());
		val2.Add("value", (JToken)(object)TypeInt());
		val2.Add("hideFromCardopedia", (JToken)(object)TypeBool());
		val2.Add("script", (JToken)(object)TypeString());
		val.Add("properties", (JToken)val2);
		JArray val3 = new JArray();
		val3.Add(JToken.op_Implicit("id"));
		val.Add("required", (JToken)val3);
		CardBaseProps = val;
		JObject val4 = new JObject();
		val4.Add("type", JToken.op_Implicit("object"));
		JObject val5 = new JObject();
		val5.Add("$schema", (JToken)(object)TypeString());
		val5.Add("id", (JToken)(object)TypeString());
		val5.Add("nameTerm", (JToken)(object)Ref("term"));
		val5.Add("nameOverride", (JToken)(object)TypeString());
		val5.Add("group", (JToken)(object)NamesFromEnum(typeof(BlueprintGroup)));
		val5.Add("icon", (JToken)(object)TypeString("Sprite. Value must be the file name of an image in your mods Icons/ folder."));
		val5.Add("value", (JToken)(object)TypeInt());
		val5.Add("hideFromCardopedia", (JToken)(object)TypeBool());
		val5.Add("hideFromIdeasTab", (JToken)(object)TypeBool());
		val5.Add("isInvention", (JToken)(object)TypeBool());
		val5.Add("needsExactMatch", (JToken)(object)TypeBool());
		val5.Add("script", (JToken)(object)TypeString());
		JObject val6 = new JObject();
		val6.Add("type", JToken.op_Implicit("array"));
		val6.Add("items", (JToken)(object)Ref("typeSubprint"));
		val5.Add("subprints", (JToken)val6);
		val4.Add("properties", (JToken)val5);
		JArray val7 = new JArray();
		val7.Add(JToken.op_Implicit("id"));
		val4.Add("required", (JToken)val7);
		BlueprintBaseProps = val4;
		PropBlacklist = new List<string>
		{
			"CardData.Id", "CardData.descriptionOverride", "CardData.nameOverride", "CardData.NameTerm", "CardData.DescriptionTerm", "CardData.PickupSound", "CardData.UniqueId", "CardData.ParentUniqueId", "CardData.EquipmentHolderUniqueId", "CardData.Value",
			"CardData.Icon", "CardData.HideFromCardopedia", "CardData.MyGameCard", "CardData.MyCardType", "CardData.StatusEffects", "CardData.CreationMonth", "CardData.ExpectedValue", "Altar.inCutscene", "Animal.CreateTimer", "Combatable.Attacked",
			"Combatable.AttackIsHit", "Combatable.AttackTimer", "Combatable.BeingAttacked", "Combatable.CurrentAttackType", "Combatable.InAttack", "Combatable.InAttackTimer", "Combatable._combatableDescription", "Combatable.AttackTargets", "Combatable.MyConflict", "Combatable.AttackAnimations",
			"Combatable.CurrentHitText", "Combatable.BeingAttacked", "Combatable.StunTimer", "Conveyor.Direction", "Conveyor.corners", "DragonEgg.NormalIcon", "DragonEgg.CrackedIcon", "DragonEgg.CrackedIcon_2", "DragonEgg.CrackedSound", "Food.SpoilTime",
			"Mimic.TreasureChestIcon", "Mimic.RealIcon", "Mob.MoveTimer", "Mob.CurrentTarget", "Mob.moveFlag", "Poop.MakeSickTimer", "ResourceChest.SpecialIcon", "Royal.MoveTimer", "Spirit.SpiritSounds", "StrangePortal.SpawnTimer",
			"StrangePortal.TravelTimer", "TrashCan.DestroySounds", "University.InventionSound", "University.SpecialIcon", "WickedWitch.WitchDieSounds", "WickedWitch.NormalIcon", "WickedWitch.OldLadyIcon", "WishingWell.SpecialIcon", "WishingWell.WishSound", "Equipable.AttackSounds",
			"Equipable.blueprint", "Equipable._equipableInfo", "Subprint.SubprintIndex", "Subprint.ParentBlueprint", "Blueprint.BlueprintGroup", "Blueprint.HideFromIdeasTab", "Blueprint.IsInvention", "Blueprint.NeedsExactMatch", "Blueprint.Subprints"
		};
		PropOverride = new Dictionary<string, JObject>
		{
			{
				"CardBag.SetPackCards",
				Ref("cardIdArray")
			},
			{
				"Subprint.RequiredCards",
				TypeString()
			},
			{
				"Subprint.CardsToRemove",
				TypeString()
			},
			{
				"Subprint.ExtraResultCards",
				TypeString()
			},
			{
				"Subprint.ResultCard",
				Ref("cardId")
			}
		};
		TypeLookup = new Dictionary<Type, JObject>
		{
			{
				typeof(string),
				TypeString()
			},
			{
				typeof(int),
				TypeInt()
			},
			{
				typeof(float),
				TypeFloat()
			},
			{
				typeof(bool),
				TypeBool()
			},
			{
				typeof(BaitBag),
				Ref("typeBaitBag")
			},
			{
				typeof(CardBag),
				Ref("typeCardBag")
			},
			{
				typeof(CardChance),
				Ref("typeCardChance")
			},
			{
				typeof(CardPalette),
				Ref("typeCardPalette")
			},
			{
				typeof(Color),
				Ref("typeColor")
			},
			{
				typeof(CombatStats),
				Ref("typeCombatStats")
			},
			{
				typeof(Subprint),
				Ref("typeSubprint")
			},
			{
				typeof(Sprite),
				TypeString("Sprite. Value must be the file name of an image in your mods Icons/ folder.")
			}
		};
		JObject val8 = new JObject();
		val8.Add("typeColor", (JToken)(object)TypeColor());
		JObject val9 = new JObject();
		val9.Add("type", JToken.op_Implicit("object"));
		JObject val10 = new JObject();
		val10.Add("Color", (JToken)(object)Ref("typeColor"));
		val10.Add("Color2", (JToken)(object)Ref("typeColor"));
		val10.Add("Icon", (JToken)(object)Ref("typeColor"));
		val9.Add("properties", (JToken)val10);
		val8.Add("typeCardPalette", (JToken)val9);
		val8.Add("typeCombatStats", (JToken)(object)GetCombatStats());
		JObject val11 = new JObject();
		val11.Add("type", JToken.op_Implicit("object"));
		JObject val12 = new JObject();
		val12.Add("baitId", (JToken)(object)TypeString());
		val11.Add("properties", (JToken)val12);
		val8.Add("typeBaitBag", (JToken)val11);
		val8.Add("typeCardChance", (JToken)(object)GetCardChance());
		val8.Add("typeCardBag", (JToken)(object)GetCardBag());
		val8.Add("typeSubprint", (JToken)(object)GetSubprint());
		val8.Add("cardId", (JToken)(object)GetCardIds());
		JObject val13 = new JObject();
		val13.Add("type", JToken.op_Implicit("array"));
		val13.Add("items", (JToken)(object)Ref("cardId"));
		val8.Add("cardIdArray", (JToken)val13);
		val8.Add("term", (JToken)(object)GetTerms());
		Defs = val8;
	}
}
