using System;

public static class Utility {
	static public string TimeToString(long milliseconds) {
		milliseconds /= 1000;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", milliseconds / 3600, (milliseconds % 3600 / 60), milliseconds % 3600 % 60);
	}

	static public string Concat(params object[] objects) {
		string ans = string.Empty, sep = string.Empty;
		foreach (object obj in objects) {
			ans += sep + obj.ToString();
			sep = "|";
		}

		return ans;
	}
}
