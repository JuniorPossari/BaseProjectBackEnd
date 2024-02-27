using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace BaseProject.Util
{
	public static class Cryptography
    {
		public static string CreateMD5(string base64)
		{
			using var md5 = MD5.Create();

			var bytes = Convert.FromBase64String(base64);

			var hashBytes = md5.ComputeHash(bytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));

			return sb.ToString();
		}

		public static string CreateMD5(byte[] bytes)
		{
			using var md5 = MD5.Create();

			var hashBytes = md5.ComputeHash(bytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));

			return sb.ToString();
		}

		public static string CreateSHA256(string base64)
		{
			using var sha256 = SHA256.Create();

			var bytes = Convert.FromBase64String(base64);

			var hashBytes = sha256.ComputeHash(bytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));

			return sb.ToString();
		}

		public static string CreateSHA256(byte[] bytes)
		{
			using var sha256 = SHA256.Create();

			var hashBytes = sha256.ComputeHash(bytes);

			var sb = new StringBuilder();

			for (int i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));

			return sb.ToString();
		}
	}
}
