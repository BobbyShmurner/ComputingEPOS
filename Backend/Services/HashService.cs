using System.Security.Cryptography;

namespace ComputingEPOS.Backend.Services;

public class HashService : IHashService {
	private const int HashSize = 20;
	private const int HashIter = 10000;

	public byte[] GenerateSaltedHash(string password, byte[] salt) {
		using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, HashIter, HashAlgorithmName.SHA256)) {
			return rfc2898.GetBytes(HashSize);
		}
	}
}