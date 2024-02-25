using MCM.Abstractions.Base.Global;
using SpawnMoreClans.Settings;
using TaleWorlds.CampaignSystem;

public void OnDailyTick()
{
    //IL_0012: Unknown result type (might be due to invalid IL or missing references)
    //IL_0017: Unknown result type (might be due to invalid IL or missing references)
    if (CanSpawnClans())
    {
        _smcData.LastSpawned = CampaignTime.Now;
        for (int i = 0; i < GlobalSettings<MCMSettings>.Instance.NumberOfClansToSpawnAtOnce; i++)
        {
            AttemptToGenerateNewClan();
        }
    }
}
