namespace ComputingEPOS.Common;

public static class StringExtensions {
	/// <summary>
	/// Trims the specified string from the start of the string.
	/// </summary>
	/// <param name="trimString">The string to trim from the start</param>
	/// <returns>The trimmed string</returns>
	public static string TrimStart(this string str, string trimString) {
		if (string.IsNullOrEmpty(trimString)) return str;

		string result = str;
		while (result.StartsWith(trimString)) {
			result = result.Substring(trimString.Length);
		}

		return result;
	}

	/// <summary>
	/// Trims the specified string from the end of the string.
	/// </summary>
	/// <param name="trimString">The string to trim from the end</param>
	/// <returns>The trimmed string</returns>
	public static string TrimEnd(this string str, string trimString) {
		if (string.IsNullOrEmpty(trimString)) return str;

		string result = str;
		while (result.EndsWith(trimString)) {
			result = result.Substring(0, result.Length - trimString.Length);
		}

		return result;
	}
}