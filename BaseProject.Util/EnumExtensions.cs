using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BaseProject.Util
{
	public static class EnumExtensions
    {
		public static string GetDisplayName(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.Name
				  ?? val.ToString();
		}

		public static string GetDisplayShortName(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.ShortName
				  ?? val.ToString();
		}

		public static string GetDisplayDescription(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.Description
				  ?? val.ToString();
		}

		public static string GetDisplayPrompt(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.Prompt
				  ?? val.ToString();
		}

		public static string GetDisplayGroupName(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.GroupName
				  ?? val.ToString();
		}

		public static int GetDisplayOrder(this Enum val)
		{
			return val.GetType()
				  .GetMember(val.ToString())
				  .FirstOrDefault()
				  ?.GetCustomAttribute<DisplayAttribute>(false)
				  ?.Order
				  ?? -1;
		}

        public static T GetValueFromDisplayName<T>(string name)
        {
            var type = typeof(T);

            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null && attribute.Name.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
				else if (field.Name.ToLower().RemoveDiacritics() == name.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
			}

            return default;
        }

        public static T GetValueFromDisplayShortName<T>(string shortname)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
				var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attribute != null && attribute.ShortName.ToLower().RemoveDiacritics() == shortname.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
				else if (field.Name.ToLower().RemoveDiacritics() == shortname.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
			}

            return default;
        }

		public static T GetValueFromDisplayDescription<T>(string description)
		{
			var type = typeof(T);

			if (!type.IsEnum) throw new InvalidOperationException();

			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attribute != null && attribute.Description.ToLower().RemoveDiacritics() == description.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
				else if (field.Name.ToLower().RemoveDiacritics() == description.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
			}

			return default;
		}

		public static T GetValueFromDisplayPrompt<T>(string prompt)
		{
			var type = typeof(T);

			if (!type.IsEnum) throw new InvalidOperationException();

			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attribute != null && attribute.Prompt.ToLower().RemoveDiacritics() == prompt.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
				else if (field.Name.ToLower().RemoveDiacritics() == prompt.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
			}

			return default;
		}

		public static T GetValueFromDisplayGroupName<T>(string groupName)
		{
			var type = typeof(T);

			if (!type.IsEnum) throw new InvalidOperationException();

			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attribute != null && attribute.GroupName.ToLower().RemoveDiacritics() == groupName.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
				else if (field.Name.ToLower().RemoveDiacritics() == groupName.ToLower().RemoveDiacritics()) return (T)field.GetValue(null);
			}

			return default;
		}

		public static T GetValueFromDisplayOrder<T>(int order)
		{
			var type = typeof(T);

			if (!type.IsEnum) throw new InvalidOperationException();

			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attribute != null && attribute.Order == order) return (T)field.GetValue(null);
			}

			return default;
		}
	}
}
