using SpawnMoreClans.SaveData;
using TaleWorlds.CampaignSystem;

public override void SyncData(IDataStore dataStore)
{
    dataStore.SyncData<SMCData>("SpawnMoreClans_Data", ref _smcData);
}
