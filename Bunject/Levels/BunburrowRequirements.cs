namespace Bunject.Levels
{
  public class BunburrowRequirements
  {
    public int Bunnies { get; set; }
    public int Babies { get; set; }
    public int HomeCaptures { get; set; }

    public bool AreMet(GeneralProgression progression)
    {
      return progression.HistoryCapturedBunnies.Count >= Bunnies
          && progression.ExistingCouples.Count >= Babies
          && progression.HomeCapturedBunnies.Count >= HomeCaptures;
    }
  }
}
