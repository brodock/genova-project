using System;
using Server;

namespace Server.MegaSpawnerSystem
{
	public enum Setting
	{
		OverrideIndividualEntries,
		AddItem,
		AddContainer
	}

	public enum Access
	{
		MasterAdmin,
		HeadAdmin,
		Admin,
		User,
		None
	}

	public enum Process
	{
		None,
		LoadBackup,
		SaveBackup,
		ConvertPre3_2
	}

	public enum LoadType
	{
		Manual,
		FileBrowserMsf,
		FileBrowserMbk,
		Error
	}

	public enum SaveType
	{
		FromFileMenu,
		Workspace,
		FileEdit
	}

	public enum SpawnType
	{
		Regular,
		Proximity,
		GameTimeBased,
		RealTimeBased,
		Speech
	}

	public enum TimerType
	{
		Master,
		Personal,
		RandomEntryDelay
	}

	public enum StyleType
	{
		OriginalBlack,
		DistroGray,
		Marble,
		BlackBorder,
		GrayEmbroidered,
		Offwhite,
		OffwhiteBorder,
		GrayMultiBorder,
		DarkGray,
		Scroll
	}

	public enum BackgroundType
	{
		Alpha,
		BlackAlpha,
		Black,
		Gray,
		Marble,
		Offwhite,
		DarkGray,
		Scroll,
		ActiveTextEntry,
		InactiveTextEntry
	}

	public enum TextColor
	{
		LightPurple,
		Purple,
		DarkPurple,
		LightBlue,
		Blue,
		DarkBlue,
		LightRed,
		Red,
		DarkRed,
		LightGreen,
		Green,
		DarkGreen,
		LightYellow,
		Yellow,
		DarkYellow,
		White,
		LightGray,
		MediumLightGray,
		MediumGray,
		DarkGray,
		Black
	}

	public enum FromGump
	{
		None,
		AddViewSettings,
		EditSpawn,
		AddItemsToContainer
	}

	public enum SearchType
	{
		Any,
		MobileType,
		ItemType
	}

	public enum SortSearchType
	{
		None,
		Name,
		Facet,
		Location,
		LocationX,
		LocationY,
		LocationZ,
		EntryName,
		DupeSpawner
	}

	public enum SortOrder
	{
		Ascending,
		Descending
	}
}