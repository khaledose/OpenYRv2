using OpenRA.Mods.Common.FileFormats;
using OpenRA.Primitives;
using System.Text;

namespace OpenRA.Mods.Ra2.UtilityCommands;

sealed class ImportLegacyTilesetCommand : IUtilityCommand
{
	string IUtilityCommand.Name => "--tileset-ra2-import";

	bool IUtilityCommand.ValidateArguments(string[] args)
	{
		return args.Length >= 3;
	}

	[Desc("FILENAME", "TEMPLATEEXTENSION", "[TILESETNAME]", "Convert a legacy tileset to the OpenRA format.")]
	void IUtilityCommand.Run(Utility utility, string[] args)
	{
		var modData = Game.ModData = utility.ModData;
		var file = new IniFile(File.Open(args[1], FileMode.Open));
		var extension = args[2];
		var tileSize = utility.ModData.Manifest.Get<MapGrid>().TileSize;
		var name = args.Length > 3 ? args[3] : Path.GetFileNameWithoutExtension(args[2]);

		var terrainTypes = InitializeTerrainTypes();
		var metadata = new StringBuilder();
		var data = new StringBuilder();

		BuildMetadata(metadata, name);
		BuildTemplate(file, extension, modData, terrainTypes, tileSize, data, out var usedCategories);
		FinalizeMetadata(metadata, usedCategories);
		BuildTerrainTypes(metadata, terrainTypes);

		Console.Write(metadata.ToString());
		Console.Write(data.ToString());
	}

	static List<TerrainTypeInfo> InitializeTerrainTypes()
	{
		return new List<TerrainTypeInfo>
		{
			new()
			{
				Type = "Clear",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "Clear",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			}, // Note: sometimes "Ice"
            new()
			{
				Type = "Ice",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			},
			new()
			{
				Type = "Ice",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			},
			new()
			{
				Type = "Ice",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			},
			new()
			{
				Type = "Road",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			}, // TS defines this as "Tunnel", but we don't need this
            new()
			{
				Type = "Rail",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "Impassable",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			}, // TS defines this as "Rock", but also uses it for buildings
            new()
			{
				Type = "Impassable",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			},
			new()
			{
				Type = "Water",
				TargetTypes = new() { "Water", "WaterTerrain" },
			},
			new()
			{
				Type = "Beach",
				TargetTypes = new() { "Water", "WaterTerrain" },
			}, // TS defines this as "Beach", but uses it for water...?
            new()
			{
				Type = "Road",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "DirtRoad",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			}, // TS defines this as "Road", but we may want different speeds
            new()
			{
				Type = "Clear",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			},
			new()
			{
				Type = "Rough",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "Cliff",
				TargetTypes = new() { "Ground", "GroundTerrain" },
			}, // TS defines this as "Rock"
            new()
			{
				Type = "Tree",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				MinColors = new() { "6F9F7F" },
				MaxColors = new() { "6F9F7F" }
			},
			new()
			{
				Type = "Rock",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				MinColors = new() { "60608C" },
				MaxColors = new() { "60608C" }
			},
			new()
			{
				Type = "Ore",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				MinColors = new() { "F0C000" },
				MaxColors = new() { "F0C000" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "Gems",
				TargetTypes = new() { "Ground", "GroundTerrain" },
				MinColors = new() { "600060" },
				MaxColors = new() { "600060" },
				SmudgeTypes = new() { "SmallCrater", "MediumCrater", "LargeCrater", "SmallScorch", "MediumScorch", "LargeScorch" }
			},
			new()
			{
				Type = "Jumpjet",
				MinColors = new() { "C7C9FA" },
				MaxColors = new() { "C7C9FA" }
			}
		};
	}

	static void BuildMetadata(StringBuilder metadata, string name)
	{
		metadata.AppendLine("General:");
		metadata.AppendLine($"\tName: {name}");
		metadata.AppendLine($"\tId: {name.ToUpperInvariant()}");
		metadata.AppendLine("\tHeightDebugColors: 00000080, 00004480, 00008880, 0000CC80, 0000FF80, 4400CC80," +
			" 88008880, CC004480, FF110080, FF550080, FF990080, FFDD0080, DDFF0080, 99FF0080, 55FF0080, 11FF0080");
	}

	static void FinalizeMetadata(StringBuilder metadata, HashSet<string> usedCategories)
	{
		metadata.AppendLine($"\tEditorTemplateOrder: {string.Join(", ", usedCategories)}");
		metadata.AppendLine("\tSheetSize: 2048");
		metadata.AppendLine("\tEnableDepth: true");
		metadata.AppendLine();
	}

	static void BuildTemplate(IniFile file, string extension, ModData modData,
		List<TerrainTypeInfo> terrainTypes, Size tileSize, StringBuilder data,
		out HashSet<string> usedCategories)
	{
		var definedCategories = new HashSet<string>();
		usedCategories = new HashSet<string>();
		var templateIndex = 0;

		data.AppendLine("Templates:");

		try
		{
			for (var tilesetGroupIndex = 0; ; tilesetGroupIndex++)
			{
				var section = file.GetSection($"TileSet{tilesetGroupIndex:D4}");
				var sectionCount = Exts.ParseInt32Invariant(section.GetValue("TilesInSet", "1"));
				var sectionFilename = section.GetValue("FileName", "").ToLowerInvariant();
				var sectionCategory = section.GetValue("SetName", "");
				if (!string.IsNullOrEmpty(sectionCategory) && sectionFilename != "blank")
					definedCategories.Add(sectionCategory);

				for (var i = 1; i <= sectionCount; i++, templateIndex++)
				{
					var templateFilename = $"{sectionFilename}{i:D2}.{extension}";
					if (!modData.DefaultFileSystem.Exists(templateFilename))
						continue;

					using (var s = modData.DefaultFileSystem.Open(templateFilename))
					{
						data.AppendLine($"\tTemplate@{templateIndex}:");
						data.AppendLine($"\t\tCategories: {sectionCategory}");
						usedCategories.Add(sectionCategory);

						data.AppendLine($"\t\tId: {templateIndex}");

						var images = GetTemplateImages(sectionFilename, extension, modData, i);
						data.AppendLine($"\t\tImages: {string.Join(", ", images)}");

						var templateWidth = s.ReadUInt32();
						var templateHeight = s.ReadUInt32();
						s.ReadInt32(); // tileWidth
						s.ReadInt32(); // tileHeight
						var offsets = new uint[templateWidth * templateHeight];
						for (var j = 0; j < offsets.Length; j++)
							offsets[j] = s.ReadUInt32();

						data.AppendLine($"\t\tSize: {templateWidth}, {templateHeight}");
						data.AppendLine("\t\tTiles:");

						BuildTemplateTiles(s, offsets, terrainTypes, tileSize, data, templateFilename);
					}
				}
			}
		}
		catch (InvalidOperationException)
		{
			// GetSection will throw when we run out of sections to import
		}
	}

	static void BuildTemplateTiles(Stream s, uint[] offsets, List<TerrainTypeInfo> terrainTypes,
		Size tileSize, StringBuilder data, string templateFilename)
	{
		for (var j = 0; j < offsets.Length; j++)
		{
			if (offsets[j] == 0)
				continue;

			s.Position = offsets[j] + 40;
			var height = s.ReadUInt8();
			var terrainType = s.ReadUInt8();
			var rampType = s.ReadUInt8();

			if (terrainType >= terrainTypes.Count)
				throw new InvalidDataException($"Unknown terrain type {terrainType} in {templateFilename}");

			var terrainTypeName = terrainTypes[terrainType].Type;
			data.AppendLine($"\t\t\t{j}: {terrainTypeName}");
			if (height != 0)
				data.AppendLine($"\t\t\t\tHeight: {height}");

			if (rampType != 0)
				data.AppendLine($"\t\t\t\tRampType: {rampType}");

			var minColor = $"{s.ReadUInt8():X2}{s.ReadUInt8():X2}{s.ReadUInt8():X2}";
			var maxColor = $"{s.ReadUInt8():X2}{s.ReadUInt8():X2}{s.ReadUInt8():X2}";
			data.AppendLine($"\t\t\t\tMinColor: {minColor}");
			data.AppendLine($"\t\t\t\tMaxColor: {maxColor}");

			terrainTypes[terrainType].MinColors.Add(minColor);
			terrainTypes[terrainType].MaxColors.Add(maxColor);

			//var zOffset = -tileSize.Height / 2.0f;
			var zOffset = 0;
			data.AppendLine($"\t\t\t\tZOffset: {zOffset}");
			data.AppendLine("\t\t\t\tZRamp: 0");
		}
	}

	static void BuildTerrainTypes(StringBuilder metadata, List<TerrainTypeInfo> terrainTypes)
	{
		metadata.AppendLine("Terrain:");
		foreach (var terrainInfo in terrainTypes.OrderByDescending(t => t.MinColors.Count).DistinctBy(t => t.Type))
		{
			metadata.AppendLine($"\tTerrainType@{terrainInfo.Type}:");
			metadata.AppendLine($"\t\tType: {terrainInfo.Type}");
			if (terrainInfo.TargetTypes.Count > 0)
				metadata.AppendLine($"\t\tTargetTypes: {string.Join(", ", terrainInfo.TargetTypes)}");
			metadata.AppendLine($"\t\tColor: {DetermineAverageColor(terrainInfo.MinColors, terrainInfo.MaxColors)}");
			if (terrainInfo.SmudgeTypes.Count > 0)
				metadata.AppendLine($"\t\tAcceptsSmudgeTypes: {string.Join(", ", terrainInfo.SmudgeTypes)}");
		}
		metadata.AppendLine();
	}

	static List<string> GetTemplateImages(string sectionFilename, string extension, ModData modData, int index)
	{
		var images = new List<string> { $"{sectionFilename}{index:D2}.{extension}" };
		for (var v = 'a'; v <= 'z'; v++)
		{
			var variant = $"{sectionFilename}{index:D2}{v}.{extension}";
			if (modData.DefaultFileSystem.Exists(variant))
				images.Add(variant);
		}
		return images;
	}

	static string DetermineAverageColor(List<string> minColors, List<string> maxColors)
	{
		if (minColors.Count == 0 || maxColors.Count == 0)
			return "000000";

		var allColors = minColors.Concat(maxColors).ToList();
		var (r, g, b) = (0, 0, 0);

		foreach (var color in allColors)
		{
			r += Convert.ToInt32(color[..2], 16);
			g += Convert.ToInt32(color.Substring(2, 2), 16);
			b += Convert.ToInt32(color.Substring(4, 2), 16);
		}

		r /= allColors.Count;
		g /= allColors.Count;
		b /= allColors.Count;

		return $"{r:X2}{g:X2}{b:X2}";
	}

	class TerrainTypeInfo
	{
		public string Type { get; set; }
		public List<string> TargetTypes { get; set; } = new();
		public List<string> MinColors { get; set; } = new();
		public List<string> MaxColors { get; set; } = new();
		public List<string> SmudgeTypes { get; set; } = new();
	}
}
