namespace ComputingEPOS.Backend.Services;

public interface IHashService {
	byte[] GenerateSaltedHash(string password, byte[] salt);
}