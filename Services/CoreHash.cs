namespace CoreContable.Services;

public interface ICoreHash {
    string Hash(string input);

    bool Verify(string input, string hash);
}

public class CoreHash(
    ILogger<CoreHash> logger
) : ICoreHash {
    public string Hash(string input) {
        try {
            // TODO: Implementar un salt dinámico
            // var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            // return BCrypt.Net.BCrypt.HashPassword(input, salt);
            return BCrypt.Net.BCrypt.HashPassword(input);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CoreHash), nameof(Hash));
            return "";
        }
    }

    public bool Verify(string input, string hash) {
        try {
            // decrypt with salt
            return BCrypt.Net.BCrypt.Verify(input, hash);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CoreHash), nameof(Verify));
            // bool newTry = BCrypt.Net.BCrypt.Verify(input, hash);
            return false;
        }
    }
}