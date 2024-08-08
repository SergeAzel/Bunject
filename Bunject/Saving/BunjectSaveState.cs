using Bunburrows;
using Levels;
using Saving.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Saving
{
  public class BunjectSaveState
  {
    public BunjectSaveState()
    {
      CustomBunburrows = new List<BunjectSaveBunburrow>();

    }

    public List<BunjectSaveBunburrow> CustomBunburrows { get; set; }
  }

  public class BunjectSaveBunburrow
  {
    public string Name { get; set; }

    public string Indicator { get; set; }

    public int ID { get; set; }
  }

  public class BunjectProgressSaveData
  {
    public List<BunjectLevelIdentitySaveData> LevelsCompletedOnce { get; set; }

    public List<BunjectLevelIdentitySaveData> LevelsVisited { get; set; }

    public List<BunjectBunnyIdentitySaveData> SeenBunnies { get; set; }

    public List<BunjectBunnyIdentitySaveData> CapturedBunnies { get; set; }

    public List<BunjectBunnyIdentitySaveData> HistoryCapturedBunnies { get; set; }

    public List<BunjectBunnyIdentitySaveData> HomeCapturedBunnies { get; set; }

    public List<BunjectBunnyPairSaveData> ExistingCouples { get; set; }
  }

  public class BunjectLevelIdentitySaveData
  {
    public int BunburrowID { get; set; }
    public int Depth { get; set; }

    public LevelIdentitySaveData ToSaveData(SaveConverter converter)
    {
      if (converter.BunburrowExists(BunburrowID))
      {
        return new LevelIdentitySaveData(converter.ConvertBunburrow(BunburrowID), Depth);
      }
      return null;
    }
  }

  public class BunjectBunnyIdentitySaveData
  {
    public int BunburrowID { get; set; }
    public int InitialDepth { get; set; }
    public int LevelID { get; set; }
    public int SpriteSheetID { get; set; }

    public BunnyIdentitySaveData ToSaveData(SaveConverter converter)
    {
      if (converter.BunburrowExists(BunburrowID))
      {
        return new BunnyIdentitySaveData(converter.ConvertBunburrow(BunburrowID), InitialDepth, LevelID, SpriteSheetID);
      }
      return null;
    }
  }

  public class BunjectBunnyPairSaveData
  {
    public BunjectBunnyIdentitySaveData Left { get; set; }
    public BunjectBunnyIdentitySaveData Right { get; set; }

    public PairSaveData<BunnyIdentitySaveData> ToSaveData(SaveConverter converter)
    {
      if (Left?.ToSaveData(converter) is BunnyIdentitySaveData left && Right.ToSaveData(converter) is BunnyIdentitySaveData right)
      {
        return new PairSaveData<BunnyIdentitySaveData>(left, right);
      }
      return null;
    }
  }
}
