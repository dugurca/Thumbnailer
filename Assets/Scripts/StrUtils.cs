using System.Text;

public static class StrUtils
{
	private static readonly StringBuilder Sb = new StringBuilder();

	public static string Add(params string[] strings)
	{
		Sb.Length = 0;
		foreach (var s in strings)
		{
			Sb.Append(s);
		}

		return Sb.ToString();
	}
}