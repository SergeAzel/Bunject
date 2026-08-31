namespace Bunject.Computer
{
  public interface ICustomPageGenerator
  {
    TPage CreateComputerPage<TPage>() where TPage : BasicCustomComputerPageController;
  }
}
