using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Template.Extensions;

namespace Template.Framework.Hypermedia
{

	public enum HttpMethod
	{
		GET,
		POST,
		DELETE,
		PUT
	}

	public class EnumSupportingProperty : SupportingProperty
	{
		public EnumSupportingProperty()
		{
			Range = "Enum";
		}

		public EnumSupportingProperty(string enumName)
		{
			Property = enumName;
		}

		public EnumSupportingProperty(string enumName, string defaultValue)
		{
			Property = enumName;
			DefaultValue = defaultValue;
		}
	}

	public class BooleanSupportingProperty : SupportingProperty
	{
		public BooleanSupportingProperty()
		{
			Range = "Boolean";
		}

		public BooleanSupportingProperty(string name)
		{
			Property = name;
		}

		public BooleanSupportingProperty(string name, bool defaultValue)
		{
			Property = name;
			DefaultValue = defaultValue.ToString();
		}
	}


	public class DataSupportingProperty : SupportingProperty
	{
		public DataSupportingProperty()
		{
			Range = "Data";
		}

		public static DataSupportingProperty FromEnum<T>(string name, string defaultValue) where T : struct, IConvertible
		{
			return new DataSupportingProperty
			{
				Property = name,
				DefaultValue = defaultValue,
				Data = EnumExtensions.AsValueDescriptionList<T>().ToList().Select(x => new HydraClass(x.Value.ToString(), x.Description)).ToList()
			};
		}

		public DataSupportingProperty(string name)
		{
			Property = name;
		}

		public DataSupportingProperty(string name, IEnumerable<HydraClass> data) : this(name)
		{
			Data = data.ToList();
		}

		public DataSupportingProperty(string name, IEnumerable<HydraClass> data, string defaultValue) : this(name, data)
		{
			DefaultValue = defaultValue;
		}
	}

	public class IntegerSupportingProperty : SupportingProperty
	{
		public IntegerSupportingProperty()
		{
			Range = "Integer";
		}

		public IntegerSupportingProperty(string name)
		{
			Property = name;
		}

		public IntegerSupportingProperty(string name, string defaultValue)
		{
			Property = name;
			DefaultValue = defaultValue;
		}
	}

	public class ArraySupportingProperty : SupportingProperty
	{
		public ArraySupportingProperty()
		{
			Range = "Array";
		}

		public ArraySupportingProperty(string name)
		{
			Property = name;
		}

		public ArraySupportingProperty(string name, string defaultValue)
		{
			Property = name;
			DefaultValue = defaultValue;
		}
	}

	public class TextSupportingProperty : SupportingProperty
	{
		public TextSupportingProperty()
		{
			Range = "Text";
		}

		public TextSupportingProperty(string name)
		{
			Property = name;
		}

		public TextSupportingProperty(string name, string defaultValue)
		{
			Property = name;
			DefaultValue = defaultValue;
		}
	}

	public class SupportingProperty
	{
		public string Property { get; set; }
		public string DefaultValue { get; set; }
		public string Range { get; set; }
		public List<HydraClass> Data { get; set; }

		public SupportingProperty()
		{
			Range = "text";
			Property = "";
			DefaultValue = "";
			Data = new List<HydraClass>();
		}

		public SupportingProperty(string property) : this()
		{
			Property = property;
		}

		public SupportingProperty(string property, string defaultValue) : this()
		{
			DefaultValue = defaultValue;
		}

	}

	public class HydraExpects
	{
		[JsonProperty(PropertyName = "supportedProperty")]
		public List<SupportingProperty> SupportingPropertyList { get; set; }

		public HydraExpects()
		{
			SupportingPropertyList = new List<SupportingProperty>();
		}
	}

	public class HydraOperation
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public HttpMethod Method { get; set; }

		public HydraExpects Expects { get; set; }

		public HydraOperation()
		{
			Method = HttpMethod.GET;
			Expects = new HydraExpects();
		}

		public SupportingProperty GetProperty(string name)
		{
			return Expects.SupportingPropertyList.FirstOrDefault(x => x.Property == name);
		}

		public void AddPropery(SupportingProperty property)
		{
			Expects.SupportingPropertyList.Add(property);
		}

		public void AddTextPropery(string name)
		{
			Expects.SupportingPropertyList.Add(new SupportingProperty(name));
		}

		public void AddFilePropery(string name)
		{
			Expects.SupportingPropertyList.Add(new SupportingProperty { Property = name, Range = "File" });
		}

	}


	public class HydraContext
	{
		public string Vocab = "https://schema.org/";
		public string Hydra = "http://www.w3.org/ns/hydra/core#";
	}

	public class HydraRepresentation : HydraClass
	{
		[JsonProperty(Order = 1)]
		public HydraContext Context { get; set; }

		public HydraRepresentation()
		{
			Context = new HydraContext();
		}
	}


	public class HydraValue : HydraClassLight
	{
		[JsonProperty(Order = 1, PropertyName = "@value")]
		public string Value { get; set; }

		public HydraValue()
		{
			Value = "";
			Type = "";
		}

		public HydraValue(string type, string value)
		{
			Type = type;
			Value = value;
		}
	}

	public class HydraLink
	{
		[JsonProperty(Order = 1, PropertyName = "@id")]
		public string Id { get; set; }

		[JsonProperty(Order = 2, PropertyName = "@type")]
		public string Type { get; set; }

		[JsonProperty(Order = 3, PropertyName = "dcterms:title")]
		public string Title { get; set; }

		public HydraLink()
		{
			Id = "";
			Type = "";
			Title = "";
		}

		public HydraLink(string id, string type) : this()
		{
			Id = id;
			Type = type;
		}

		public HydraLink(string id, string type, string title) : this()
		{
			Id = id;
			Type = type;
			Title = title;
		}

		public string HTMLId()
		{
			return Id.Replace("/", "-");
		}

		public bool Empty()
		{
			return Id.Empty();
		}

		public bool NotEmpty()
		{
			return !Empty();
		}

		public HydraLink WithTitle(string title)
		{
			Title = title;
			return this;
		}

		public string Path()
		{
			if (Id.Contains("?"))
				return Id.Substring(0, Id.IndexOf("?") + 1);

			return Id;
		}
	}

	public class HydraClassLight
	{
		[JsonProperty(Order = 1, PropertyName = "@id")]
		public string Id { get; set; }

		[JsonProperty(Order = 2, PropertyName = "dcterms:title")]
		public string Title { get; set; }

		[JsonProperty(Order = 3, PropertyName = "@type")]
		public string Type { get; set; }

		public HydraClassLight()
		{

		}

		public HydraClassLight(string id, string title)
		{
			Id = id;
			Title = title;
		}
	}

	public class HydraClass : HydraClassLight
	{

		[JsonProperty(Order = 5, PropertyName = "operation")]
		public List<HydraOperation> OperationList { get; set; }

		public List<string> ErrorMessages { get; set; }

		[JsonIgnore]
		public string HumanAlert { get; set; }

		[JsonIgnore]
		public bool Empty { get; set; }

		public HydraClass()
		{
			OperationList = new List<HydraOperation>();
			Type = GetType().Name.Replace("Hypermedia", "");
			ErrorMessages = new List<string>();
			Title = "";
			Empty = true;
			HumanAlert = "";
		}

		public HydraClass(string id, string title)
		{
			Id = id;
			Title = title;
		}

		public string FirstErrorMessage()
		{
			if (!HasErrors()) return "";

			return ErrorMessages.FirstOrDefault();
		}

		public bool HasHumanAlert()
		{
			return HumanAlert.NotEmpty();
		}

		public T WithHumanAlert<T>(string alert) where T : HydraClass
		{
			HumanAlert = alert;
			return this as T;
		}

		public void AddErrorMessage(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				ErrorMessages.Add(message);
			}
		}

		public string HTMLId()
		{
			return Id.Replace("/", "-");
		}

		public int IntegerId()
		{

			if (string.IsNullOrEmpty(Id))
				return -1;

			if (!Id.Contains('/'))
				return -1;

			return Id.Substring(Id.LastIndexOf('/') + 1).ToInt();
		}

		public bool HasErrors()
		{
			return ErrorMessages.Any();
		}

		public void AddPost(string textField)
		{
			AddPost(new[] { new TextSupportingProperty(textField) });
		}

		public void AddPost(string[] textFields)
		{
			AddPost(textFields.ToList().Select(x => new TextSupportingProperty(x)));
		}

		public void AddEmptyPost()
		{
			var operation = new HydraOperation { Method = HttpMethod.POST };

			OperationList.Add(operation);
		}

		public void AddPost(IEnumerable<SupportingProperty> properties)
		{
			var operation = new HydraOperation { Method = HttpMethod.POST };

			properties.ToList().ForEach(operation.AddPropery);

			OperationList.Add(operation);
		}

		public void AddPut(TextSupportingProperty prop)
		{
			AddPut(new[] { prop });
		}

		public void AddPut(BooleanSupportingProperty prop)
		{
			AddPut(new[] { prop });
		}


		public void AddPut(string[] textFields)
		{
			AddPut(textFields.ToList().Select(x => new TextSupportingProperty(x)));
		}

		public void AddPut(IEnumerable<SupportingProperty> properties)
		{
			var operation = new HydraOperation { Method = HttpMethod.PUT };

			properties.ToList().ForEach(operation.AddPropery);

			OperationList.Add(operation);
		}

		public void AddEmptyPut()
		{
			var operation = new HydraOperation { Method = HttpMethod.PUT };

			OperationList.Add(operation);
		}

		public void AddDelete()
		{
			var operation = new HydraOperation { Method = HttpMethod.DELETE };

			OperationList.Add(operation);
		}

		public void AddGet()
		{
			var operation = new HydraOperation { Method = HttpMethod.GET };

			OperationList.Add(operation);
		}


		public void AddGet(IEnumerable<SupportingProperty> properties)
		{
			var operation = new HydraOperation { Method = HttpMethod.GET };

			properties.ToList().ForEach(operation.AddPropery);

			OperationList.Add(operation);
		}

		public void RemoveDelete()
		{
			var delete = GetDelete();
			if (delete != null)
				OperationList.Remove(delete);
		}

		public void RemovePost()
		{
			var op = GetPost();
			if (op != null)
				OperationList.Remove(op);
		}

		public void RemovePut()
		{
			var op = GetPut();
			if (op != null)
				OperationList.Remove(op);
		}

		public void RemoveGet()
		{
			var op = GetGet();
			if (op != null)
				OperationList.Remove(op);
		}

		public bool CanPut()
		{
			return GetPut() != null;
		}

		public bool CanPost()
		{
			return GetPost() != null;
		}

		public bool CanDelete()
		{
			return GetDelete() != null;
		}

		public bool CanGet()
		{
			return GetGet() != null;
		}

		public HydraOperation GetGet()
		{
			return GetOperation(HttpMethod.GET);
		}

		public HydraOperation GetPut()
		{
			return GetOperation(HttpMethod.PUT);
		}

		public HydraOperation GetPost()
		{
			return GetOperation(HttpMethod.POST);
		}

		public HydraOperation GetDelete()
		{
			return GetOperation(HttpMethod.DELETE);
		}

		public HydraOperation GetOperation(HttpMethod method)
		{
			return OperationList.FirstOrDefault(x => x.Method == method);
		}

		public HydraLink GetLink()
		{
			if (CanGet())
				return new HydraLink(Id, Type);

			return new HydraLink();
		}

		public HydraLink ToLink()
		{
			return new HydraLink(Id, Type, Title);
		}
	}

	public class HydraLinkCollection : HydraClass
	{
		public List<HydraLink> Members { get; set; }

		public HydraLinkCollection()
		{
			Type = "@Collection";
			Members = new List<HydraLink>();
		}

		public bool HasMembers()
		{
			return Members.Any();
		}

		public bool EmptyMembers()
		{
			return !Members.Any();
		}

		public void Add(HydraLink item)
		{
			Members.Add(item);
		}
	}

	public class HydraCollection<T> : HydraClass where T : HydraClassLight
	{
		public List<T> Members { get; set; }

		public HydraCollection()
		{
			Type = "@Collection";
			Members = new List<T>();
		}

		public bool HasMembers()
		{
			return Members.Any();
		}


		public bool EmptyMembers()
		{
			return !Members.Any();
		}

		public void Add(T item)
		{
			Members.Add(item);
		}
	}


	public class HydraStringCollection
	{
		private string Type { get; set; }
		public List<string> Members { get; set; }

		public HydraStringCollection()
		{
			Type = "@Collection";
			Members = new List<string>();
		}
	}

}
