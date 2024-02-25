using MCM.Abstractions.Base.Global;
using SpawnMoreClans.SaveData;
using SpawnMoreClans.Settings;
using TaleWorlds.CampaignSystem;

public bool CanSpawnClans()
{
    if (_smcData == null)
    {
        _smcData = new SMCData();
    }
    return (int)((CampaignTime)(ref _smcData.LastSpawned)).ElapsedDaysUntilNow >= GlobalSettings<MCMSettings>.Instance.DaysBeforeNewClansCanSpawn;
}
