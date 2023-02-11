
using System.Collections.Generic;
using System.Numerics;

namespace NVMP
{
	public enum WorldspaceType : uint
	{
		/// <summary>
		/// Invalid worldspace
		/// </summary>
		None = 0,

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		FFEncounterWorld = 0x00031E12,
		FXInvertedDaylightWorld = 0x000B3625,
		TestMap01 = 0x000D703C,
		WastelandNV = 0x000DA726,
		FreesideWorld = 0x0010BEEA,
		GamorrahWorld = 0x00110628,
		GreenhouseWorld01 = 0x0012A914,
		FreesideNorthWorld = 0x0012D94D,
		FreesideFortWorld = 0x0012D94E,
		TheStripWorldNew = 0x0013B308,
		TheFortWorld = 0x0013C0B6,
		WastelandNVmini = 0x00148C05,
		BoulderCityWorld = 0x0014C723,
		Lucky38World = 0x0016D714,

		NVDLC03BigMT = 0xFF000E81,
		NVDLC03Higgs = 0xFF00C6FA,

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}

	public class WorldspaceBounds
	{
		public WorldspaceBounds(Vector2 min, Vector2 max)
		{
			Max = max;
			Min = min;
		}

		public Vector2 Min { get; }
		public Vector2 Max { get; }
	}

	/// <summary>
	/// A cell coordinate in Fallout
	/// </summary>
	public class WorldspaceCoordinate
    {
		/// <summary>
		/// The worldspace formID 
		/// </summary>
		public WorldspaceType FormID { get; internal set; }

		/// <summary>
		/// X coordinate
		/// </summary>
		public int X { get; internal set; }

		/// <summary>
		/// Y coordinate
		/// </summary>
		public int Y { get; internal set; }
	}

	public static class WorldspaceUtility
	{
		public static WorldspaceType AsModIndex(this WorldspaceType type, byte index)
		{
			uint newVal = (((uint)type << 8) >> 8) | ((uint)index << 24);
			return (WorldspaceType)newVal;
		}

		public static WorldspaceType AsDefined(this WorldspaceType type)
		{
			uint newVal = (((uint)type << 8) >> 8) | ((uint)0xff << 24);
			return (WorldspaceType)newVal;
		}

		public static readonly IDictionary<WorldspaceType, WorldspaceBounds> Bounds = new Dictionary<WorldspaceType, WorldspaceBounds>
		{
			{ WorldspaceType.WastelandNV, new WorldspaceBounds
				(
					min: new Vector2 { X = -174879.52f, Y = -133428.89f },
					max: new Vector2 { X = 137077.88f, Y = 179414.52f }
				)
			},
			{ WorldspaceType.NVDLC03BigMT, new WorldspaceBounds
				(
					min: new Vector2 { X= -19839.037f, Y = -24239.42f },
					max: new Vector2 { X = -11133.824f, Y = 4957.7666f }
				)
			}
		};
	}

}