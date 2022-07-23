namespace NVMP.Entities
{
    public enum NetReferenceFormType : uint
    {
		None = 0,
		FileHeader,
		Group,
		GameSetting,
		TextureSet,
		MenuIcon,
		Global,
		Class,
		Faction,
		HeadPart,
		Hair,
		Eyes,
		Race,
		Sound,
		AcousticSpace,
		Skill,
		BaseEffect,
		Script,
		LandTexture,
		ObjectEffect,
		ActorEffect,
		Activator,
		TalkingActivator,
		Terminal,
		Armor,
		Book,
		Clothing,
		Container,
		Door,
		Ingredient,
		Light,
		Misc,
		Static,
		StaticCollection,
		MoveableStatic,
		PlaceableWater,
		Grass,
		Tree,
		Flora,
		Furniture,
		Weapon,
		Ammo,
		NPC,
		Creature,
		LeveledCreature,
		LeveledCharacter,
		Key,
		Ingestible,
		IdleMarker,
		Note,
		ConstructibleObject,
		Projectile,
		LeveledItem,
		Weather,
		Climate,
		Region,
		NavmeshInfoMap,
		Cell,
		Reference,
		CharacterReference,
		CreatureReference,
		MissleProjectile,
		GrenadeProjectile,
		BeamProjectile,
		FlameProjectile,
		Worldspace,
		Land,
		Navmesh,
		TLOD,
		DialogTopic,
		DialogTopicInfo,
		Quest,
		Idle,
		Package,
		CombatStyle,
		LoadScreen,
		LeveledSpell,
		AnimatedObject,
		Water,
		EffectShader,
		OffsetTable,
		Explosion,
		Debris,
		ImageSpace,
		ImageSpaceModifier,
		FormList,
		Perk,
		BodyPartData,
		AddonNode,
		ActorValueInfo,
		RadiationStage,
		CameraShot,
		CameraPath,
		VoiceType,
		ImpactData,
		ImpactDataSet,
		ArmorAddon,
		EncounterZone,
		Message,
		Ragdoll,
		DefaultObjectManager,
		LightingTemplate,
		Music,
		WeaponMod,
		Reputation,
		ContinuousBeamProjectile,
		Recipe,
		RecipeCategory,
		CasinoChip,
		Casino,
		LoadScreenType,
		MediaSet,
		MediaLocationController,
		Challenge,
		AmmoEffect,
		CaravanCard,
		CaravanMoney,
		CaravanDeck,
		DehydrationStage,
		HungerStage,
		SleepDeprivationStage,
	}

    public static class FormTypeHelpers
    {
		public static bool IsInventoryType(this NetReferenceFormType type)
		{
			switch (type)
			{
				case NetReferenceFormType.Weapon:
				case NetReferenceFormType.Ammo:
				case NetReferenceFormType.Armor:
				case NetReferenceFormType.Book:
				case NetReferenceFormType.Clothing:
				case NetReferenceFormType.Ingredient:
				case NetReferenceFormType.Light:
				case NetReferenceFormType.Misc:
				case NetReferenceFormType.Key:
				case NetReferenceFormType.Ingestible:
				case NetReferenceFormType.Note:
				case NetReferenceFormType.ConstructibleObject:
				case NetReferenceFormType.LeveledItem:
				case NetReferenceFormType.WeaponMod:
				case NetReferenceFormType.CasinoChip:
				case NetReferenceFormType.CaravanCard:
				case NetReferenceFormType.CaravanMoney:
					return true;
				default:
					return false;
			}
		}
	}
}
