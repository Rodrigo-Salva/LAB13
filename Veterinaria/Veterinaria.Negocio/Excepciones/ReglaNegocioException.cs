namespace Veterinaria.Negocio.Excepciones;

/// <summary>Se lanza cuando una operación incumple una regla de negocio (el API la traduce a 409).</summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}
