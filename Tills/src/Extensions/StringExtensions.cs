namespace ComputingEPOS.Tills;

public static class StringExtensions {
	public static string TrimStart(this string str, string trimString) {
		if (string.IsNullOrEmpty(trimString)) return str;

		string result = str;
		while (result.StartsWith(trimString)) {
			result = result.Substring(trimString.Length);
		}

		return result;
	}

	public static string TrimEnd(this string str, string trimString) {
		if (string.IsNullOrEmpty(trimString)) return str;

		string result = str;
		while (result.EndsWith(trimString)) {
			result = result.Substring(0, result.Length - trimString.Length);
		}

		return result;
	}
}